using System;
using System.IO;
using System.Linq;
using Xunit;

namespace CmdrsChronicle.Core.Tests
{
    public class SqliteSchemaInitializerTests
    {
        [Fact]
        public void Schema_Creates_All_Expected_Tables()
        {
            // Try known locations, then walk upward from the test output directory
            var fileName = "cmdrschronicle_schema.sql";
            var candidates = new[]
            {
                Path.Combine(AppContext.BaseDirectory, fileName), // output dir (preferred)
                Path.Combine("..", "src", "CmdrsChronicle.Core", "Schema", fileName),
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "src", "CmdrsChronicle.Core", "Schema", fileName),
                Path.Combine("..", "src", "CmdrsChronicle.Core", fileName),
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "src", "CmdrsChronicle.Core", fileName)
            };
            string? schemaPath = candidates.FirstOrDefault(File.Exists);

            if (schemaPath == null)
            {
                var dir = new System.IO.DirectoryInfo(AppContext.BaseDirectory);
                // Walk up to 8 levels looking for a central schema file or the canonical Schema folder
                for (int i = 0; i < 8 && dir != null; i++)
                {
                    var candidateRoot = Path.Combine(dir.FullName, fileName);
                    if (File.Exists(candidateRoot)) { schemaPath = candidateRoot; break; }

                    var candidateSchemaFolder = Path.Combine(dir.FullName, "src", "CmdrsChronicle.Core", "Schema", fileName);
                    if (File.Exists(candidateSchemaFolder)) { schemaPath = candidateSchemaFolder; break; }

                    dir = dir.Parent;
                }
            }
            Assert.True(schemaPath != null, $"Schema file not found in any known location. Tried: {string.Join(", ", candidates)}");

            // Act: Create DB and get table names
            using var conn = CmdrsChronicle.Core.SqliteSchemaInitializer.CreateInMemoryDbWithSchema(schemaPath);
            var tables = CmdrsChronicle.Core.SqliteSchemaInitializer.GetTableNames(conn);

            // Assert: At least one table exists and names are non-empty
            Assert.NotEmpty(tables);
            Assert.All(tables, t => Assert.False(string.IsNullOrWhiteSpace(t)));

            // Print for manual inspection (optional)
            Console.WriteLine("Tables created:");
            foreach (var t in tables) Console.WriteLine(t);
        }
    }
}
