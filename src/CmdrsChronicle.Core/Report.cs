using System;
using System.Collections.Generic;
using System.IO;

namespace CmdrsChronicle.Core
{
    /// <summary>
    /// Utility methods for report generation. The rendering pipeline uses this class to
    /// select a random tagline from the <c>taglines.txt</c> file.
    /// </summary>
    public class Report
    {
        /// <summary>
        /// Selects a random non-comment tagline from the taglines.txt file.
        /// Lines beginning with <c>#</c> and blank lines are skipped.
        /// </summary>
        /// <param name="taglinesPath">Path to the taglines.txt file.</param>
        /// <returns>A random tagline string, or <see langword="null"/> if no valid lines are found.</returns>
        public static string? SelectRandomTagline(string taglinesPath)
        {
            if (!File.Exists(taglinesPath)) return null;
            var lines = File.ReadAllLines(taglinesPath);
            var taglines = new List<string>();
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#")) continue;
                taglines.Add(trimmed);
            }
            if (taglines.Count == 0) return null;
            // Random.Shared is a thread-safe singleton — no need to create a new Random() instance.
            return taglines[Random.Shared.Next(taglines.Count)];
        }
    }
}
