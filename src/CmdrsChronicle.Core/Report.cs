using System;
using System.Collections.Generic;

namespace CmdrsChronicle.Core
{
    /// <summary>
    /// Represents a generated report.
    /// </summary>
        public class Report
        {
            /// <summary>
            /// Selects a random non-comment tagline from the taglines.txt file.
            /// </summary>
            /// <param name="taglinesPath">Path to the taglines.txt file.</param>
            /// <returns>A random tagline string, or null if none found.</returns>
            public static string SelectRandomTagline(string taglinesPath)
            {
                if (!System.IO.File.Exists(taglinesPath)) return null;
                var lines = System.IO.File.ReadAllLines(taglinesPath);
                var taglines = new List<string>();
                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#")) continue;
                    taglines.Add(trimmed);
                }
                if (taglines.Count == 0) return null;
                var rng = new Random();
                return taglines[rng.Next(taglines.Count)];
            }

            /// <summary>Title of the report.</summary>
            public string Title { get; set; }
            /// <summary>Date and time the report was generated.</summary>
            public DateTime DateGenerated { get; set; }
            /// <summary>Style of the report ("elegant" or "colorful").</summary>
            public string Style { get; set; }
            /// <summary>Sections included in the report.</summary>
            public List<ReportSection> Sections { get; set; } = new();
            /// <summary>Embedded assets (base64 or inline).</summary>
            public List<string> EmbeddedAssets { get; set; } = new();
        }
}
