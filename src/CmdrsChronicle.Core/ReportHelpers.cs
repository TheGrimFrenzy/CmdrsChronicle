using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CmdrsChronicle.Core
{
    /// <summary>
    /// Miscellaneous helpers shared across report renderers.
    /// </summary>
    public static class ReportHelpers
    {
        /// <summary>
        /// Converts a real-world <see cref="DateTime"/> to its Elite Dangerous lore date string.
        /// Elite lore years are exactly 1286 years ahead of the real-world calendar,
        /// so January 2026 becomes January 3312.
        /// </summary>
        /// <param name="dt">The real-world date to convert.</param>
        /// <returns>A date string in the format <c>YYYY-MM-DD</c> using the lore year.</returns>
        public static string FormatLoreDate(DateTime dt)
        {
            var loreYear = dt.Year + 1286;
            return $"{loreYear:D4}-{dt.Month:D2}-{dt.Day:D2}";
        }

// RegexOptions.Compiled converts the pattern to IL at class-load time;
		// the overhead is paid once and each subsequent match is faster.
		private static readonly Regex _journalFileRegex = new Regex(
            @"^Journal\.(\d{4}-\d{2}-\d{2}T\d{6})\.(\d{2})\.log$",
            RegexOptions.Compiled);

        /// <summary>
        /// Parses a raw timestamp string to a lore-year date. Tries full ISO 8601 parsing
        /// first, then falls back to the date-only prefix, then returns the prefix verbatim.
        /// Returns <see langword="null"/> when <paramref name="tsStr"/> is null, empty, or shorter than 10 characters.
        /// </summary>
        public static string? ParseLoreDate(string? tsStr)
        {
            if (string.IsNullOrEmpty(tsStr)) return null;
            if (DateTime.TryParse(tsStr, out var parsedTs))
                return FormatLoreDate(parsedTs);
            if (tsStr.Length >= 10 && DateTime.TryParse(tsStr[..10], out var parsedShort))
                return FormatLoreDate(parsedShort);
            if (tsStr.Length >= 10)
                return tsStr[..10];
            return null;
        }

        /// <summary>
        /// Scans journal files in the given directory to find the most recent star system
        /// visited before (but not on) the specified boundary date.
        /// Useful for seeding the report header with the commander's last known location.
        /// </summary>
        /// <param name="directory">Directory containing <c>Journal.*.log</c> files.</param>
        /// <param name="boundary">Exclusive upper date boundary; only events <em>before</em> this date are considered.
        /// Pass <see langword="null"/> to scan all available journal files.</param>
        /// <returns>
        /// A tuple of (<c>lastSystem</c>, <c>lastDate</c>, <c>cmdrName</c>).
        /// Fields default to <c>"Unknown System"</c>, <c>"Unknown Date"</c>, and <c>"Unknown Commander"</c>
        /// when no matching data is found.
        /// </returns>
        public static (string lastSystem, string lastDate, string cmdrName) FindMostRecentStarSystem(string directory, DateTime? boundary)
        {
            var lastSystem = "Unknown System";
            var lastDate = "Unknown Date";
            var cmdrName = "Unknown Commander";
            bool foundSystem = false, foundCmdr = false;

            if (!Directory.Exists(directory)) return (lastSystem, lastDate, cmdrName);

            var files = JournalFileDiscovery.DiscoverJournalFiles(directory);
            var candidateFiles = new List<(string path, DateTime dt)>();

            foreach (var f in files)
            {
                var fn = Path.GetFileName(f);
                var m = _journalFileRegex.Match(fn);
                if (!m.Success) continue;
                var dtStr = m.Groups[1].Value + "." + m.Groups[2].Value;
                if (!DateTime.TryParseExact(dtStr, "yyyy-MM-ddTHHmmss.ff", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var dt))
                    continue;
                if (boundary.HasValue && dt >= boundary.Value) continue;
                candidateFiles.Add((f, dt));
            }

            candidateFiles.Sort((a, b) => b.dt.CompareTo(a.dt)); // latest first

            foreach (var (filePath, fileDt) in candidateFiles)
            {
                try
                {
                    foreach (var line in File.ReadLines(filePath).Reverse())
                    {
                        try
                        {
                            using var doc = JsonDocument.Parse(line);
                            var root = doc.RootElement;

                            // Pick up the most recent StarSystem (reading in reverse, so first hit is latest)
                            if (!foundSystem &&
                                root.TryGetProperty("StarSystem", out var s) && s.ValueKind == JsonValueKind.String)
                            {
                                var systemName = s.GetString();
                                if (!string.IsNullOrEmpty(systemName))
                                {
                                    foundSystem = true;
                                    lastSystem = systemName;
                                    if (root.TryGetProperty("timestamp", out var ts) && ts.ValueKind == JsonValueKind.String)
                                    {
                                        var parsed = ParseLoreDate(ts.GetString());
                                        if (parsed != null) lastDate = parsed;
                                    }
                                }
                            }

                            // Commander only appears on LoadGame events (at session start, so near the
                            // beginning of the file — which means near the end when reading in reverse).
                            if (!foundCmdr &&
                                root.TryGetProperty("event", out var evtProp) &&
                                evtProp.GetString() == "LoadGame" &&
                                root.TryGetProperty("Commander", out var c) && c.ValueKind == JsonValueKind.String)
                            {
                                var found = c.GetString();
                                if (!string.IsNullOrWhiteSpace(found))
                                {
                                    foundCmdr = true;
                                    cmdrName = found;
                                }
                            }

                            if (foundSystem && foundCmdr)
                                return (lastSystem, lastDate, cmdrName);
                        }
                        catch { }
                    }
                }
                catch { }

                // Found the system in this file; no need to search older files unless cmdrName is still unknown
                if (foundSystem && foundCmdr)
                    return (lastSystem, lastDate, cmdrName);
            }

            return (lastSystem, lastDate, cmdrName);
        }
    }
}
