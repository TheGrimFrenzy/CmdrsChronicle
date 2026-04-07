using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CmdrsChronicle.SchemaGen
{
    /// <summary>
    /// Generates SQLite <c>CREATE TABLE</c> statements from Elite Dangerous event JSON schema files.
    /// Handles nested objects and arrays by creating child tables with foreign-key references.
    /// </summary>
    public class SchemaGenerator
    {
        /// <summary>
        /// Generates a single CREATE TABLE SQL statement for the primary event table only,
        /// intentionally discarding any child-table statements that would result from nested
        /// object/array properties. Use <see cref="GenerateCreateTableSqlWithChildren"/> when
        /// you need the full set of tables.
        /// </summary>
        public string GeneratePrimaryTableSql(EventSchema schema, HashSet<string> reservedWords)
        {
            // Use the event schema title as the table name.
            var stmts = GenerateCreateTableSqlWithChildren(schema, reservedWords, schema.Title);
            return stmts.Count > 0 ? stmts[0] : string.Empty;
        }

        /// <summary>
        /// Deserializes all <c>*.json</c> schema files in the given directory into
        /// <see cref="EventSchema"/> objects.
        /// </summary>
        /// <param name="schemaDirectory">Directory containing Elite Dangerous event schema JSON files.</param>
        /// <returns>List of deserialized schemas; files that fail to deserialize are silently skipped.</returns>
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

        /// <summary>
        /// Generates one or more SQLite <c>CREATE TABLE IF NOT EXISTS</c> statements for the given
        /// schema: the first statement is for the primary event table, followed by statements for any
        /// child tables derived from nested object or array-of-object properties.
        /// Reserved SQL keywords are prefixed with <c>event_</c> to avoid conflicts.
        /// </summary>
        /// <param name="schema">The parsed event schema to convert.</param>
        /// <param name="reservedWords">Set of lowercase SQLite reserved words used to escape column/table names.</param>
        /// <param name="tableName">Desired SQL table name for the primary table.</param>
        /// <returns>Ordered list of SQL statements: primary table first, child tables after.</returns>
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
                        Title = childName,
                        Properties = new Dictionary<string, System.Text.Json.JsonElement>(),
                        Required = new List<string>()
                    };
                    // Copy child properties
                    if (itemsElem.TryGetProperty("properties", out var childPropsElem) && childPropsElem.ValueKind == System.Text.Json.JsonValueKind.Object)
                    {
                        foreach (var childProp in childPropsElem.EnumerateObject())
                        {
                            childSchema.Properties[childProp.Name] = childProp.Value;
                        }
                    }
                    // Copy required
                    if (itemsElem.TryGetProperty("required", out var childReqElem) && childReqElem.ValueKind == System.Text.Json.JsonValueKind.Array)
                    {
                        foreach (var req in childReqElem.EnumerateArray())
                        {
                            if (req.ValueKind == System.Text.Json.JsonValueKind.String)
                                childSchema.Required.Add(req.GetString());
                        }
                    }
                    childTables.Add((childName, childSchema));
                    // Do NOT add this property as a column in parent
                    continue;
                }
                // Detect singular object property for child table (1:1 relationship)
                if (prop.Value.ValueKind == System.Text.Json.JsonValueKind.Object &&
                    prop.Value.TryGetProperty("type", out var objTypeElem) &&
                    objTypeElem.GetString() == "object" &&
                    prop.Value.TryGetProperty("properties", out var objPropsElem) &&
                    objPropsElem.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    var childName = tableName + "_" + prop.Key;
                    var childSchema = new EventSchema
                    {
                        Title = childName,
                        Properties = new Dictionary<string, System.Text.Json.JsonElement>(),
                        Required = new List<string>()
                    };
                    foreach (var childProp in objPropsElem.EnumerateObject())
                    {
                        childSchema.Properties[childProp.Name] = childProp.Value;
                    }
                    if (prop.Value.TryGetProperty("required", out var objReqElem) && objReqElem.ValueKind == System.Text.Json.JsonValueKind.Array)
                    {
                        foreach (var req in objReqElem.EnumerateArray())
                        {
                            if (req.ValueKind == System.Text.Json.JsonValueKind.String)
                                childSchema.Required.Add(req.GetString());
                        }
                    }
                    childTables.Add((childName, childSchema));
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
            var safeTableName = EscapeName(tableName, reservedWords);
            sqlStatements.Add($"CREATE TABLE IF NOT EXISTS {safeTableName} (\n    {string.Join(",\n    ", columns)}\n);\n");
            // Generate child tables
            foreach (var (childName, childSchema) in childTables)
            {
                var safeChildName  = EscapeName(childName, reservedWords);
                var safeParentName = EscapeName(tableName, reservedWords);
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

        // Prefix column/table names that collide with SQLite reserved words to avoid SQL syntax errors.
        private string EscapeName(string name, HashSet<string> reservedWords)
        {
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

    /// <summary>
    /// Data-transfer object representing one Elite Dangerous event JSON schema file.
    /// <c>[JsonPropertyName]</c> attributes map the lowercase JSON keys to idiomatic
    /// PascalCase C# properties so the class reads naturally in code.
    /// </summary>
    public class EventSchema
    {
        /// <summary>Event name / table name as declared in the JSON schema <c>"title"</c> field.</summary>
        [JsonPropertyName("title")]
        public string Title      { get; set; } = string.Empty;

        /// <summary>Map of property name to its JSON schema descriptor element.</summary>
        [JsonPropertyName("properties")]
        public Dictionary<string, JsonElement> Properties { get; set; } = new();

        /// <summary>List of property names that are marked as required in the schema.</summary>
        [JsonPropertyName("required")]
        public List<string> Required { get; set; } = new();
    }
}
