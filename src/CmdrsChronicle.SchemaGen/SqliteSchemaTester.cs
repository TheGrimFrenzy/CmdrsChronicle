using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace CmdrsChronicle.SchemaGen
{
    /// <summary>
    /// Validates generated SQLite <c>CREATE TABLE</c> statements by executing them against
    /// an in-memory SQLite database. Used as a smoke-test during schema generation.
    /// </summary>
    public static class SqliteSchemaTester
    {
        /// <summary>
        /// Executes each SQL statement in sequence against a fresh in-memory SQLite database.
        /// Stops at the first failure and returns a diagnostic message; returns <c>"OK"</c> on success.
        /// </summary>
        /// <param name="sqlStatements">Ordered sequence of <c>CREATE TABLE</c> SQL strings to execute.</param>
        /// <returns><c>"OK"</c> if all statements executed successfully, or a failure description
        /// including the statement index and SQLite error message.</returns>
        public static string TryCreateTablesSequentially(IEnumerable<string> sqlStatements)
        {
            using var conn = new SqliteConnection("Data Source=:memory:");
            conn.Open();
            int idx = 0;
            foreach (var sql in sqlStatements)
            {
                idx++;
                try
                {
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    return $"FAIL at statement #{idx}:\n{sql}\nError: {ex.Message}";
                }
            }
            return "OK";
        }
    }
}
