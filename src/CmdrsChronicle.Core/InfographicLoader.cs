using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CmdrsChronicle.Core
{
    /// <summary>
    /// Discovers and deserializes infographic tile definitions from JSON files
    /// under the /infographics/{Category}/ directory tree.
    /// </summary>
    public static class InfographicLoader
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Loads all enabled infographic definitions from *.json files under
        /// <paramref name="infographicsRoot"/>. Malformed or disabled files are silently skipped.
        /// </summary>
        /// <param name="infographicsRoot">Root directory containing category subdirectories.</param>
        public static List<InfographicDefinition> LoadAll(string infographicsRoot)
        {
            var results = new List<InfographicDefinition>();
            if (!Directory.Exists(infographicsRoot))
                return results;

            foreach (var file in Directory.GetFiles(infographicsRoot, "*.json", SearchOption.AllDirectories))
            {
                InfographicDefinition? def;
                try
                {
                    var json = File.ReadAllText(file);
                    def = JsonSerializer.Deserialize<InfographicDefinition>(json, _options);
                }
                catch { continue; }

                if (def == null || !def.Enabled) continue;
                def.SourceFile = file;
                results.Add(def);
            }

            return results;
        }
    }
}
