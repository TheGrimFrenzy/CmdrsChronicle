using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CmdrsChronicle.SchemaGen;

namespace SchemaGenTool
{
    class Program
    {
        static readonly string githubApiBase = "https://api.github.com/repos/jixxed/ed-journal-schemas/contents/schemas";
        static readonly string githubRawBase = "https://raw.githubusercontent.com/jixxed/ed-journal-schemas/main/schemas";


        static async Task Main(string[] args)
        {
            string outputSql = args.Length > 0 ? args[0] : "cmdrschronicle_schema.sql";
            string githubToken = args.Length > 1 ? args[1] : null;
            var reservedWords = new HashSet<string> { "order", "group", "select", "table", "index", "from", "where", "join", "user", "by", "as", "on", "and", "or", "not", "null", "primary", "key", "integer", "real", "text", "to" };
            var generator = new SchemaGenerator();
            var schemas = new List<(string name, EventSchema schema)>();


            // Try to load schemas from local directory first
            string localSchemaDir = "D:/Elite/CmdrsChronicleDotNet/src/external/schemas";
            if (Directory.Exists(localSchemaDir))
            {
                foreach (var file in Directory.GetFiles(localSchemaDir, "*.json", SearchOption.AllDirectories))
                {
                    var json = File.ReadAllText(file);
                    var schema = JsonSerializer.Deserialize<EventSchema>(json);
                    if (schema != null)
                        schemas.Add((Path.GetFileNameWithoutExtension(file), schema));
                }
                if (schemas.Count > 0)
                {
                    Console.WriteLine($"Loaded {schemas.Count} schemas from {localSchemaDir}");
                }
                else
                {
                    Console.WriteLine($"No schemas found in {localSchemaDir}, falling back to GitHub fetch.");
                }
            }
            if (schemas.Count == 0)
            {
                using var http = new HttpClient();
                http.DefaultRequestHeaders.UserAgent.ParseAdd("CmdrsChronicleSchemaGen/1.0");
                if (!string.IsNullOrWhiteSpace(githubToken))
                    http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", githubToken);

                async Task FetchSchemasRecursive(string apiUrl, string rawUrlPrefix)
                {
                    var json = await http.GetStringAsync(apiUrl);
                    var items = JsonSerializer.Deserialize<List<GitHubItem>>(json);
                    foreach (var item in items)
                    {
                        if (item.type == "dir")
                        {
                            await FetchSchemasRecursive($"https://api.github.com/repos/jixxed/ed-journal-schemas/contents/{item.path}", $"https://raw.githubusercontent.com/jixxed/ed-journal-schemas/main/{item.path}");
                        }
                        else if (item.type == "file" && item.name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                        {
                            if (item.name.Equals("Event.json", StringComparison.OrdinalIgnoreCase))
                                continue; // skip base Event schema
                            string rawUrl = $"{rawUrlPrefix}/{item.name}";
                            try
                            {
                                var schemaJson = await http.GetStringAsync(rawUrl);
                                var schema = JsonSerializer.Deserialize<EventSchema>(schemaJson);
                                if (schema != null)
                                    schemas.Add((Path.GetFileNameWithoutExtension(item.name), schema));
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Failed to fetch or parse {rawUrl}: {ex.Message}");
                            }
                        }
                    }
                }
                await FetchSchemasRecursive(githubApiBase, githubRawBase);
            }

            // Sort for deterministic output
            schemas = schemas.OrderBy(s => s.name).ToList();

            var sqlLines = new List<string>();
            sqlLines.Add("-- CmdrsChronicle static SQLite schema");
            sqlLines.Add("-- Generated from ED Journal schemas (https://github.com/jixxed/ed-journal-schemas/tree/main/schemas)");
            sqlLines.Add("-- DO NOT EDIT BY HAND\n");

            // Show output for one parent and its child tables as example
            bool exampleShown = false;
            var allStmts = new List<string>();
            foreach (var (name, schema) in schemas)
            {
                var stmts = generator.GenerateCreateTableSqlWithChildren(schema, reservedWords, name);
                sqlLines.AddRange(stmts);
                sqlLines.Add("");
                allStmts.AddRange(stmts);
                // Show example output for first parent with child tables
                if (!exampleShown && stmts.Count > 1)
                {
                    Console.WriteLine($"Example table output for {name}:");
                    Console.WriteLine(stmts[0]);
                    for (int i = 1; i < stmts.Count; i++)
                    {
                        Console.WriteLine(stmts[i]);
                    }
                    exampleShown = true;
                }
            }
            // Test each CREATE TABLE statement in-memory as we go
            var testResult = SqliteSchemaTester.TryCreateTablesSequentially(allStmts);
            if (testResult != "OK")
            {
                Console.WriteLine("\n[SCHEMA TEST FAILURE]");
                Console.WriteLine(testResult);
            }
            else
            {
                Console.WriteLine("[SCHEMA TEST SUCCESS] All tables created in-memory.");
            }
            File.WriteAllLines(outputSql, sqlLines);
            Console.WriteLine($"Wrote {schemas.Count} CREATE TABLE statements to {outputSql}");
        }

        // (Removed duplicate GitHubItem class)

        private class GitHubItem
        {
            public string name { get; set; }
            public string path { get; set; }
            public string type { get; set; } // "file" or "dir"
        }
    }
}
