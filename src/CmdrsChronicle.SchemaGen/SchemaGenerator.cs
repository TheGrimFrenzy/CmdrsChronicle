// ...existing code...
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CmdrsChronicle.SchemaGen
{
    public class SchemaGenerator
    {
        /// <summary>
        /// Generates a single CREATE TABLE SQL statement for the main event table (no children).
        /// Provided for test compatibility.
        /// </summary>
        public string GenerateCreateTableSql(EventSchema schema, HashSet<string> reservedWords)
        {
            // Use the event schema title as the table name
            var stmts = GenerateCreateTableSqlWithChildren(schema, reservedWords, schema.title);
            // Return only the main table statement (first)
            return stmts.Count > 0 ? stmts[0] : string.Empty;
        }

        public List<EventSchema> LoadSchemas(string schemaDirectory)
        {
            var schemas = new List<EventSchema>();
            foreach (var file in Directory.GetFiles(schemaDirectory, "*.json"))
            {
                var json = File.ReadAllText(file);
                var schema = JsonSerializer.Deserialize<EventSchema>(json);
                if (schema != null)
                    schemas.Add(schema);
            }
            return schemas;
        }

        public List<string> GenerateCreateTableSqlWithChildren(EventSchema schema, HashSet<string> reservedWords, string tableName)
        {
            var sqlStatements = new List<string>();
            var columns = new List<string>
            {
                "event_id INTEGER PRIMARY KEY AUTOINCREMENT"
            };
            bool hasEventTimestamp = false;
            var childTables = new List<(string childName, EventSchema childSchema)>();
            foreach (var prop in schema.Properties)
            {
                // Detect array-of-object for child tables
                if (prop.Value.ValueKind == System.Text.Json.JsonValueKind.Object &&
                    prop.Value.TryGetProperty("type", out var typeElem) &&
                    typeElem.GetString() == "array" &&
                    prop.Value.TryGetProperty("items", out var itemsElem) &&
                    itemsElem.ValueKind == System.Text.Json.JsonValueKind.Object &&
                    itemsElem.TryGetProperty("type", out var itemTypeElem) &&
                    itemTypeElem.GetString() == "object")
                {
                    // Child table: <Parent>_<Property>
                    var childName = tableName + "_" + prop.Key;
                    // Build child schema from itemsElem
                    var childSchema = new EventSchema
                    {
                        title = childName,
                        properties = new Dictionary<string, System.Text.Json.JsonElement>(),
                        required = new List<string>()
                    };
                    // Copy child properties
                    if (itemsElem.TryGetProperty("properties", out var childPropsElem) && childPropsElem.ValueKind == System.Text.Json.JsonValueKind.Object)
                    {
                        foreach (var childProp in childPropsElem.EnumerateObject())
                        {
                            childSchema.properties[childProp.Name] = childProp.Value;
                        }
                    }
                    // Copy required
                    if (itemsElem.TryGetProperty("required", out var childReqElem) && childReqElem.ValueKind == System.Text.Json.JsonValueKind.Array)
                    {
                        foreach (var req in childReqElem.EnumerateArray())
                        {
                            if (req.ValueKind == System.Text.Json.JsonValueKind.String)
                                childSchema.required.Add(req.GetString());
                        }
                    }
                    childTables.Add((childName, childSchema));
                    // Do NOT add this property as a column in parent
                    continue;
                }
                // Normal property
                var colName = EscapeName(prop.Key, reservedWords);
                if (colName == "event_timestamp") hasEventTimestamp = true;
                var type = MapJsonTypeToSqliteType(prop.Value);
                var notNull = schema.Required.Contains(prop.Key) ? " NOT NULL" : string.Empty;
                columns.Add($"{colName} {type}{notNull}");
            }
            // Always add event_timestamp column if not present
            if (!hasEventTimestamp)
            {
                columns.Add("event_timestamp TEXT NOT NULL");
            }
            var safeTableName = EscapeName(tableName, reservedWords, forTable: true);
            sqlStatements.Add($"CREATE TABLE IF NOT EXISTS {safeTableName} (\n    {string.Join(",\n    ", columns)}\n);\n");
            // Generate child tables
            foreach (var (childName, childSchema) in childTables)
            {
                var safeChildName = EscapeName(childName, reservedWords, forTable: true);
                var safeParentName = EscapeName(tableName, reservedWords, forTable: true);
                var childCols = new List<string>
                {
                    "child_id INTEGER PRIMARY KEY AUTOINCREMENT",
                    safeParentName + "_event_id INTEGER NOT NULL"
                };
                foreach (var prop in childSchema.Properties)
                {
                    var colName = EscapeName(prop.Key, reservedWords);
                    var type = MapJsonTypeToSqliteType(prop.Value);
                    var notNull = childSchema.Required.Contains(prop.Key) ? " NOT NULL" : string.Empty;
                    childCols.Add($"{colName} {type}{notNull}");
                }
                // Add constraints after columns
                var constraints = new List<string>
                {
                    $"FOREIGN KEY({safeParentName}_event_id) REFERENCES {safeParentName}(event_id)"
                };
                var allLines = new List<string>(childCols);
                allLines.AddRange(constraints);
                sqlStatements.Add($"    CREATE TABLE IF NOT EXISTS {safeChildName} (\n        {string.Join(",\n        ", allLines)}\n    );\n");
            }
            return sqlStatements;
        }

        private string EscapeName(string name, HashSet<string> reservedWords, bool forTable = false)
        {
            // Always prefix reserved words for both columns and tables
            var safe = name.Replace(" ", "_").Replace("-", "_");
            if (safe.StartsWith("event_"))
                return safe;
            if (reservedWords.Contains(safe.ToLowerInvariant()))
                safe = $"event_{safe}";
            return safe;
        }

        private string MapJsonTypeToSqliteType(System.Text.Json.JsonElement prop)
        {
            if (prop.ValueKind == System.Text.Json.JsonValueKind.Object && prop.TryGetProperty("type", out var typeElem))
            {
                var type = typeElem.GetString();
                return type switch
                {
                    "string" => "TEXT",
                    "number" => "REAL",
                    "integer" => "INTEGER",
                    "boolean" => "INTEGER",
                    "object" => "TEXT", // store as JSON
                    "array" => "TEXT", // store as JSON (unless handled as child table)
                    _ => "TEXT"
                };
            }
            return "TEXT";
        }
    }

    public class EventSchema
    {
        // Use property names matching JSON schema files (title, properties, required)
        public string title { get; set; } = string.Empty;
        public Dictionary<string, JsonElement> properties { get; set; } = new();
        public List<string> required { get; set; } = new();

        // Convenience properties for code
        public string Title => title;
        public Dictionary<string, JsonElement> Properties => properties;
        public List<string> Required => required;
    }
}
