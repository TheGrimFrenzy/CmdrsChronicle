using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CmdrsChronicle.Core
{
    /// <summary>
    /// Loads Elite Dangerous event schemas from a canonical local directory or repo clone.
    /// </summary>
    public static class EventSchemaLoader
    {
        /// <summary>
        /// Recursively loads all event schema JSON files from the given root directory.
        /// </summary>
        /// <param name="schemasRoot">Root directory of the canonical schemas (e.g., schemas/ from the jixxed repo).</param>
        /// <returns>Dictionary mapping event name to parsed schema JsonDocument.</returns>
        public static Dictionary<string, JsonDocument> LoadAllSchemas(string schemasRoot)
        {
            var result = new Dictionary<string, JsonDocument>(StringComparer.OrdinalIgnoreCase);
            if (!Directory.Exists(schemasRoot))
                throw new DirectoryNotFoundException($"Schema root not found: {schemasRoot}");

            foreach (var file in Directory.EnumerateFiles(schemasRoot, "*.json", SearchOption.AllDirectories))
            {
                var eventName = Path.GetFileNameWithoutExtension(file);
                var json = File.ReadAllText(file);
                var doc = JsonDocument.Parse(json);
                result[eventName] = doc;
            }
            return result;
        }
    }
}
