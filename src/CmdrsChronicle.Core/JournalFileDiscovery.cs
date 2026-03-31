using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CmdrsChronicle.Core
{
    public static class JournalFileDiscovery
    {
        private static readonly Regex JournalPattern = new(
            @"^Journal\.(\d{4}-\d{2}-\d{2})T\d{6}\.\d{2}\.log$",
            RegexOptions.Compiled);

        /// <summary>
        /// Returns all valid Elite Dangerous journal log files in the specified directory,
        /// optionally pre-filtered to only files whose filename date falls within [startDate, endDate].
        /// Journal files can span multiple days, so we keep any file whose date is &lt;= endDate and whose
        /// NEXT day (i.e. file date + 1) is &gt;= startDate — effectively any file that could contain events
        /// in range. In practice we keep files dated on or before endDate and on or after startDate-1day
        /// (one day slack because a session started the previous day may contain events crossing midnight).
        /// </summary>
        public static List<string> DiscoverJournalFiles(string directoryPath,
            DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");

            var files = Directory.EnumerateFiles(directoryPath, "Journal.*.log", SearchOption.TopDirectoryOnly)
                .Where(f => JournalPattern.IsMatch(Path.GetFileName(f)));

            if (startDate.HasValue || endDate.HasValue)
            {
                files = files.Where(f =>
                {
                    var m = JournalPattern.Match(Path.GetFileName(f));
                    if (!m.Success) return false;
                    if (!DateTime.TryParse(m.Groups[1].Value, out var fileDate)) return false;
                    // Keep file if its date is within [startDate-1day, endDate]
                    if (startDate.HasValue && fileDate < startDate.Value.Date.AddDays(-1)) return false;
                    if (endDate.HasValue   && fileDate > endDate.Value.Date) return false;
                    return true;
                });
            }

            return files.ToList();
        }

        /// <summary>
        /// Parses all journal files in parallel with a concurrency cap. Returns parsed events and error log.
        /// Optionally pre-filters files by date from the filename before parsing.
        /// </summary>
        /// <param name="directoryPath">Directory to scan for journal files.</param>
        /// <param name="maxDegreeOfParallelism">Concurrency cap.</param>
        /// <param name="startDate">Optional inclusive lower bound — files clearly before this are skipped.</param>
        /// <param name="endDate">Optional inclusive upper bound — files clearly after this are skipped.</param>
        /// <returns>(List of parsed JsonElement events, List of error log strings)</returns>
        public static (List<JsonElement> Events, List<string> Errors) ParseJournalFilesParallel(
            string directoryPath, int maxDegreeOfParallelism,
            DateTime? startDate = null, DateTime? endDate = null)
        {
            // Constitution: Parse FILES in parallel, not lines. Each file is parsed sequentially.
            var parsedEvents = new System.Collections.Concurrent.ConcurrentBag<JsonElement>();
            var errorLog = new System.Collections.Concurrent.ConcurrentBag<string>();
            var files = DiscoverJournalFiles(directoryPath, startDate, endDate);
            Console.WriteLine($"Parsing {files.Count} journal file(s) (date pre-filter applied: {startDate.HasValue || endDate.HasValue}).");
            System.Threading.Tasks.Parallel.ForEach(
                files,
                new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism },
                file =>
                {
                    try
                    {
                        foreach (var line in System.IO.File.ReadLines(file))
                        {
                            try
                            {
                                var doc = System.Text.Json.JsonDocument.Parse(line);
                                parsedEvents.Add(doc.RootElement.Clone());
                            }
                            catch (System.Text.Json.JsonException)
                            {
                                errorLog.Add($"Invalid JSON in {System.IO.Path.GetFileName(file)}: {line}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errorLog.Add($"Error reading file {System.IO.Path.GetFileName(file)}: {ex.Message}");
                    }
                });
            return (parsedEvents.ToList(), errorLog.ToList());
        }
    }
}
