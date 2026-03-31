using System;
using System.Collections.Generic;
using CmdrsChronicle.Core;
using Xunit;

namespace CmdrsChronicle.Core.Tests
{
    public class LogFileTests
    {
        [Fact]
        public void Properties_AreMappedCorrectly()
        {
            var now = DateTime.UtcNow;
            var logFile = new LogFile
            {
                Path = "/logs/Journal.2026-03-22T123456.01.log",
                Filename = "Journal.2026-03-22T123456.01.log",
                ModifiedAt = now,
                Size = 123456,
                LineCount = 789,
                Events = new System.Collections.Generic.List<System.Text.Json.JsonElement>()
            };

            Assert.Equal("/logs/Journal.2026-03-22T123456.01.log", logFile.Path);
            Assert.Equal("Journal.2026-03-22T123456.01.log", logFile.Filename);
            Assert.Equal(now, logFile.ModifiedAt);
            Assert.Equal(123456, logFile.Size);
            Assert.Equal(789, logFile.LineCount);
            Assert.NotNull(logFile.Events);
        }
    }
}
