using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace CmdrsChronicle.Core
{
    /// <summary>
    /// Runs infographic tile queries in parallel against a named shared in-memory SQLite database,
    /// using a semaphore-capped connection pool. All results are gathered before returning so that
    /// callers may filter and sort them prior to rendering.
    /// </summary>
    public static class InfographicQueryRunner
    {
        /// <summary>
        /// Runs every tile's SQL queries in parallel (pool capped by <paramref name="maxConcurrency"/>),
        /// then returns the complete result set.
        /// </summary>
        /// <param name="definitions">Tile definitions to execute.</param>
        /// <param name="dbName">Named shared in-memory SQLite database identifier.</param>
        /// <param name="startDate">
        /// Inclusive lower bound for the :startDate parameter (e.g. "2026-03-05").
        /// Pass null to omit the parameter.
        /// </param>
        /// <param name="endDate">
        /// Exclusive upper bound for the :endDate parameter (e.g. "2026-03-07").
        /// Pass null to omit the parameter.
        /// </param>
        /// <param name="maxConcurrency">Maximum simultaneous SQLite connections.</param>
        public static async Task<IReadOnlyList<InfographicQueryResult>> RunAllAsync(
            IReadOnlyList<InfographicDefinition> definitions,
            string dbName,
            string? startDate,
            string? endDate,
            int maxConcurrency)
        {
            var connString = $"Data Source={dbName};Mode=Memory;Cache=Shared";
            using var semaphore = new SemaphoreSlim(Math.Max(1, maxConcurrency));

            var tasks = new List<Task<InfographicQueryResult>>(definitions.Count);
            foreach (var def in definitions)
            {
                var captured = def;
                tasks.Add(Task.Run(async () =>
                {
                    await semaphore.WaitAsync().ConfigureAwait(false);
                    try
                    {
                        using var conn = new SqliteConnection(connString);
                        conn.Open();
                        return QueryTile(conn, captured, startDate, endDate);
                    }
                    finally { semaphore.Release(); }
                }));
            }

            return await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private static InfographicQueryResult QueryTile(
            SqliteConnection conn,
            InfographicDefinition def,
            string? startDate,
            string? endDate)
        {
            long mainValue = 0;
            var scalars = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
            var mainDuration = TimeSpan.Zero;
            var detailDuration = TimeSpan.Zero;
            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = def.Query;
                if (startDate != null) cmd.Parameters.AddWithValue(":startDate", startDate);
                if (endDate   != null) cmd.Parameters.AddWithValue(":endDate",   endDate);
                // Execute reader so we can capture multiple named scalar columns when present.
                var sw = System.Diagnostics.Stopwatch.StartNew();
                using var reader = cmd.ExecuteReader();
                sw.Stop();
                mainDuration = sw.Elapsed;
                if (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var name = reader.GetName(i) ?? ($"col{i}");
                        var valObj = reader.IsDBNull(i) ? (object?)null : reader.GetValue(i);
                        long val = 0;
                        if (valObj != null && valObj != DBNull.Value)
                        {
                            try { val = Convert.ToInt64(valObj); }
                            catch { val = 0; }
                        }
                        scalars[name] = val;
                        if (i == 0) mainValue = val;
                    }
                }
            }
            catch { /* query error → mainValue stays 0, tile excluded by threshold */ }

            var detailRows = new List<(string Label, long Value)>();
            if (!string.IsNullOrWhiteSpace(def.DetailQuery))
            {
                try
                {
                    using var detailCmd = conn.CreateCommand();
                    detailCmd.CommandText = def.DetailQuery;
                    if (startDate != null) detailCmd.Parameters.AddWithValue(":startDate", startDate);
                    if (endDate   != null) detailCmd.Parameters.AddWithValue(":endDate",   endDate);
                    var sw2 = System.Diagnostics.Stopwatch.StartNew();
                    using var reader = detailCmd.ExecuteReader();
                    sw2.Stop();
                    detailDuration = sw2.Elapsed;
                    while (reader.Read())
                    {
                        var label = reader.IsDBNull(0) ? "(unknown)" : reader.GetString(0);
                        var value = reader.IsDBNull(1) ? 0L : Convert.ToInt64(reader.GetValue(1));
                        detailRows.Add((label, value));
                    }
                }
                catch { /* detail query error → no chart data */ }
            }

            return new InfographicQueryResult
            {
                Definition = def,
                MainValue  = mainValue,
                DetailRows = detailRows,
                Scalars    = scalars,
                MainQueryDuration = mainDuration,
                DetailQueryDuration = detailDuration
            };
        }
    }
}
