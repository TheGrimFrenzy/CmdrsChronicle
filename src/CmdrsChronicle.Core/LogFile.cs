using System;
using System.Collections.Generic;
using System.Text.Json;

namespace CmdrsChronicle.Core
{
    /// <summary>
    /// Represents a single Elite Dangerous journal log file.
    /// </summary>
    public class LogFile
    {
        /// <summary>Full path to the log file.</summary>
        public string Path { get; set; }
        /// <summary>Filename (matches pattern Journal.YYYY-MM-DDThhmmss.nn.log).</summary>
        public string Filename { get; set; }
        /// <summary>Last modified timestamp.</summary>
        public DateTime ModifiedAt { get; set; }
        /// <summary>File size in bytes.</summary>
        public long Size { get; set; }
        /// <summary>Number of lines in the file.</summary>
        public int LineCount { get; set; }
        /// <summary>Events parsed from this log file.</summary>
        public List<JsonElement> Events { get; set; } = new();

        /// <summary>
        /// Loads and parses all events from the specified log file, associating them with this instance.
        /// Invalid lines are skipped; errors are returned in the error log.
        /// </summary>
        /// <param name="filePath">Path to the journal log file.</param>
        /// <returns>List of error log strings.</returns>
        public List<string> LoadEventsFromFile(string filePath)
        {
            var errorLog = new List<string>();
            var events = new List<JsonElement>();
            foreach (var line in System.IO.File.ReadLines(filePath))
            {
                try
                {
                    var doc = System.Text.Json.JsonDocument.Parse(line);
                    events.Add(doc.RootElement.Clone());
                }
                catch (System.Text.Json.JsonException)
                {
                    errorLog.Add($"Invalid JSON in {System.IO.Path.GetFileName(filePath)}: {line}");
                }
            }
            this.Events = events;
            return errorLog;
        }
    }
}
