using System;
using System.Text.Json;
using CmdrsChronicle.Core;
using Xunit;

namespace CmdrsChronicle.Core.Tests
{
    /// <summary>
    /// [DEPRECATED] Tests for LogEvent, which will be removed after event table generation (T203).
    /// </summary>
    [Obsolete("LogEventTests is deprecated and will be removed after event table generation (T203). Use schema-specific event tests instead.")]
    public class LogEventTests
    {
        [Fact]
        public void Properties_AreMappedCorrectly()
        {
            var payload = JsonDocument.Parse("{\"foo\":42}").RootElement;
            var now = DateTime.UtcNow;
            var logEvent = new LogEvent
            {
                Timestamp = now,
                EventType = "Jump",
                Payload = payload,
                System = "Sol",
                Body = "Earth",
                Planet = "Earth",
                Commander = "CMDR Test"
            };

            Assert.Equal(now, logEvent.Timestamp);
            Assert.Equal("Jump", logEvent.EventType);
            Assert.Equal(payload.ToString(), logEvent.Payload.ToString());
            Assert.Equal("Sol", logEvent.System);
            Assert.Equal("Earth", logEvent.Body);
            Assert.Equal("Earth", logEvent.Planet);
            Assert.Equal("CMDR Test", logEvent.Commander);
        }
    }
}
