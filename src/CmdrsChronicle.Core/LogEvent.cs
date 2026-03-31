using System;
using System.Text.Json;

namespace CmdrsChronicle.Core
{
    /// <summary>
    /// [DEPRECATED] Represents a single event from an Elite Dangerous journal log.
    /// This class will be removed once event tables are generated per schema (see T203).
    /// </summary>
    [Obsolete("LogEvent is deprecated and will be removed after event table generation (T203). Use schema-specific event classes instead.")]
    public class LogEvent
    {
        /// <summary>Timestamp of the event.</summary>
        public DateTime Timestamp { get; set; }
        /// <summary>Type of the event.</summary>
        public string EventType { get; set; }
        /// <summary>Raw JSON payload of the event.</summary>
        public JsonElement Payload { get; set; }
        /// <summary>System name (nullable).</summary>
        public string? System { get; set; }
        /// <summary>Body name (nullable).</summary>
        public string? Body { get; set; }
        /// <summary>Planet name (nullable).</summary>
        public string? Planet { get; set; }
        /// <summary>Commander name (nullable).</summary>
        public string? Commander { get; set; }
    }
}
