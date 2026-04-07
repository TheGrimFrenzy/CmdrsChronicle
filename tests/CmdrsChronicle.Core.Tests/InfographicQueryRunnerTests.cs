using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CmdrsChronicle.Core;
using Microsoft.Data.Sqlite;
using Xunit;

namespace CmdrsChronicle.Core.Tests
{
    public class InfographicQueryRunnerTests
    {
        private static string FindSchemaPath()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            for (int i = 0; i < 8; i++)
            {
                var candidate = Path.Combine(dir.FullName, "cmdrschronicle_schema.sql");
                if (File.Exists(candidate)) return candidate;
                var corePath = Path.Combine(dir.FullName, "src", "CmdrsChronicle.Core", "Schema", "cmdrschronicle_schema.sql");
                if (File.Exists(corePath)) return corePath;
                dir = dir.Parent ?? dir;
            }
            throw new FileNotFoundException("Cannot locate cmdrschronicle_schema.sql");
        }

        [Fact]
        public async Task RunAllAsync_ReturnsZeroMainValue_WhenTableIsEmpty()
        {
            var dbName = $"iqr_empty_{Guid.NewGuid():N}";
            using var keeper = SqliteSchemaInitializer.CreateSharedInMemoryDb(FindSchemaPath(), dbName);

            var defs = new List<InfographicDefinition>
            {
                new() {
                    Category = "Mining", Title = "Minerals Refined", Enabled = true, Threshold = 1,
                    Query = "SELECT COUNT(*) AS count FROM MiningRefined WHERE event_timestamp >= :startDate AND event_timestamp < :endDate"
                }
            };

            var results = await InfographicQueryRunner.RunAllAsync(defs, dbName, "2026-01-01", "2026-12-31", 2);

            Assert.Single(results);
            Assert.Equal(0, results[0].MainValue);
            Assert.False(results[0].MeetsThreshold);
            Assert.Empty(results[0].DetailRows);
        }

        [Fact]
        public async Task RunAllAsync_ReturnsCorrectMainValue_WhenDataExists()
        {
            var dbName = $"iqr_data_{Guid.NewGuid():N}";
            using var keeper = SqliteSchemaInitializer.CreateSharedInMemoryDb(FindSchemaPath(), dbName);

            using (var ins = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared"))
            {
                ins.Open();
                using var cmd = ins.CreateCommand();
                cmd.CommandText = """
                    INSERT INTO MiningRefined (Type, Type_Localised, event_timestamp)
                    VALUES ('Gold', 'Gold', '2026-03-05T10:00:00');
                    INSERT INTO MiningRefined (Type, Type_Localised, event_timestamp)
                    VALUES ('Platinum', 'Platinum', '2026-03-05T11:00:00');
                    """;
                cmd.ExecuteNonQuery();
            }

            var defs = new List<InfographicDefinition>
            {
                new() {
                    Category = "Mining", Title = "Minerals Refined", Enabled = true, Threshold = 1,
                    Query = "SELECT COUNT(*) AS count FROM MiningRefined WHERE event_timestamp >= :startDate AND event_timestamp < :endDate",
                    DetailQuery = "SELECT COALESCE(Type_Localised, Type) AS mineral, COUNT(*) AS refined FROM MiningRefined WHERE event_timestamp >= :startDate AND event_timestamp < :endDate GROUP BY mineral ORDER BY refined DESC",
                    ChartType = "bar-chart"
                }
            };

            var results = await InfographicQueryRunner.RunAllAsync(defs, dbName, "2026-03-05", "2026-03-06", 2);

            Assert.Single(results);
            Assert.Equal(2, results[0].MainValue);
            Assert.True(results[0].MeetsThreshold);
            Assert.Equal(2, results[0].DetailRows.Count);
        }

        [Fact]
        public async Task RunAllAsync_HandlesMultipleDefinitionsInParallel()
        {
            var dbName = $"iqr_parallel_{Guid.NewGuid():N}";
            using var keeper = SqliteSchemaInitializer.CreateSharedInMemoryDb(FindSchemaPath(), dbName);

            var defs = new List<InfographicDefinition>
            {
                new() { Category = "Mining", Title = "Refined",    Enabled = true, Threshold = 1, Query = "SELECT COUNT(*) FROM MiningRefined WHERE event_timestamp >= :startDate AND event_timestamp < :endDate" },
                new() { Category = "Mining", Title = "Prospected", Enabled = true, Threshold = 1, Query = "SELECT COUNT(*) FROM ProspectedAsteroid WHERE event_timestamp >= :startDate AND event_timestamp < :endDate" },
                new() { Category = "Mining", Title = "Cracked",    Enabled = true, Threshold = 1, Query = "SELECT COUNT(*) FROM AsteroidCracked WHERE event_timestamp >= :startDate AND event_timestamp < :endDate" },
            };

            var results = await InfographicQueryRunner.RunAllAsync(defs, dbName, "2026-01-01", "2026-12-31", 3);

            Assert.Equal(3, results.Count);
            Assert.All(results, r => Assert.Equal(0, r.MainValue));
        }

        [Fact]
        public async Task RunAllAsync_ReturnsEmpty_WhenNoDefinitions()
        {
            var dbName = $"iqr_nodefs_{Guid.NewGuid():N}";
            using var keeper = SqliteSchemaInitializer.CreateSharedInMemoryDb(FindSchemaPath(), dbName);

            var results = await InfographicQueryRunner.RunAllAsync(
                new List<InfographicDefinition>(), dbName, "2026-01-01", "2026-12-31", 2);

            Assert.Empty(results);
        }
    }
}
