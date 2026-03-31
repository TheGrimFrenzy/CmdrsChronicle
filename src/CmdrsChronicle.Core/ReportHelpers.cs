using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CmdrsChronicle.Core
{
    public static class ReportHelpers
    {
        public static string FormatLoreDate(DateTime dt)
        {
            var loreYear = dt.Year + 1286;
            return $"{loreYear:D4}-{dt.Month:D2}-{dt.Day:D2}";
        }

        // Scans journal files in directory for the most recent StarSystem before boundary (exclusive).
        // Returns tuple (lastSystem, lastDate, cmdrName) with Unknown defaults when not found.
        public static (string lastSystem, string lastDate, string cmdrName) FindMostRecentStarSystem(string directory, DateTime? boundary)
        {
            var lastSystem = "Unknown System";
            var lastDate = "Unknown Date";
            var cmdrName = "Unknown Commander";

            if (!Directory.Exists(directory)) return (lastSystem, lastDate, cmdrName);

            var files = JournalFileDiscovery.DiscoverJournalFiles(directory);
            var candidateFiles = new List<(string path, DateTime dt)>();

            foreach (var f in files)
            {
                var fn = Path.GetFileName(f);
                var m = System.Text.RegularExpressions.Regex.Match(fn, @"^Journal\.(\d{4}-\d{2}-\d{2}T\d{6})\.(\d{2})\.log$");
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
                            if (root.TryGetProperty("StarSystem", out var s) && s.ValueKind == JsonValueKind.String)
                            {
                                var foundSystem = s.GetString();
                                if (!string.IsNullOrEmpty(foundSystem))
                                {
                                    lastSystem = foundSystem;
                                    if (root.TryGetProperty("timestamp", out var ts) && ts.ValueKind == JsonValueKind.String)
                                    {
                                        var tsStr = ts.GetString();
                                        if (!string.IsNullOrEmpty(tsStr))
                                        {
                                            if (DateTime.TryParse(tsStr, out var parsedTs))
                                                lastDate = FormatLoreDate(parsedTs);
                                            else if (tsStr.Length >= 10 && DateTime.TryParse(tsStr[..10], out var parsedShort))
                                                lastDate = FormatLoreDate(parsedShort);
                                            else if (tsStr.Length >= 10)
                                                lastDate = tsStr[..10];
                                        }
                                    }
                                    if (root.TryGetProperty("Commander", out var c) && c.ValueKind == JsonValueKind.String)
                                        cmdrName = c.GetString() ?? cmdrName;

                                    return (lastSystem, lastDate, cmdrName);
                                }
                            }
                        }
                        catch { }
                    }
                }
                catch { }
            }

            return (lastSystem, lastDate, cmdrName);
        }
    }
}
