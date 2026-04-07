using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using CmdrsChronicle.SchemaGen;

namespace CmdrsChronicle.Tests
{
    public class SchemaGeneratorTests
    {
        [Fact]
        public void LoadSchemas_LoadsAllJsonFiles()
        {
            // Arrange
            var generator = new SchemaGenerator();
            var testDir = Path.Combine("TestData", "Schemas");
            Directory.CreateDirectory(testDir);
            File.WriteAllText(Path.Combine(testDir, "TestEvent.json"), "{\"title\":\"TestEvent\",\"properties\":{\"foo\":{\"type\":\"string\"}},\"required\":[\"foo\"]}");

            // Act
            var schemas = generator.LoadSchemas(testDir);

            // Assert
            Assert.Single(schemas);
            Assert.Equal("TestEvent", schemas[0].Title);
            Assert.Contains("foo", schemas[0].Properties.Keys);
        }

        [Fact]
        public void GeneratePrimaryTableSql_GeneratesValidSql()
        {
            var generator = new SchemaGenerator();
            var reserved = new HashSet<string> { "order", "group", "select" };
            var schema = new EventSchema
            {
                Title = "Order",
                Properties = new Dictionary<string, System.Text.Json.JsonElement>
                {
                    { "timestamp", System.Text.Json.JsonDocument.Parse("{\"type\":\"string\"}").RootElement },
                    { "order", System.Text.Json.JsonDocument.Parse("{\"type\":\"integer\"}").RootElement },
                    { "value", System.Text.Json.JsonDocument.Parse("{\"type\":\"number\"}").RootElement }
                },
                Required = new List<string> { "timestamp", "order" }
            };
            var sql = generator.GeneratePrimaryTableSql(schema, reserved);
            Assert.Contains("CREATE TABLE IF NOT EXISTS event_Order", sql);
            Assert.Contains("event_id INTEGER PRIMARY KEY AUTOINCREMENT", sql);
            Assert.Contains("timestamp TEXT NOT NULL", sql);
            Assert.Contains("event_order INTEGER NOT NULL", sql); // reserved word
            Assert.Contains("value REAL", sql);
        }
    }
}
