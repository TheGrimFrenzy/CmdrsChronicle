using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Xunit;

namespace CmdrsChronicle.Core.Tests
{
    public class LogFileEventLoadingTests
    {
        [Fact]
        public void LogFile_Loads_And_Associates_Parsed_Events()
        {
            // Arrange: Create a temp log file with valid and invalid lines
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            var fileName = "Journal.2026-03-21T193456.01.log";
            var filePath = Path.Combine(tempDir, fileName);
            var validLine = "{\"event\":\"Jump\",\"timestamp\":\"2026-03-21T19:34:56Z\"}";
            var invalidLine = "not a json line";
            File.WriteAllLines(filePath, new[] { validLine, invalidLine, validLine });

            var logFile = new CmdrsChronicle.Core.LogFile
            {
                Path = filePath,
                Filename = fileName,
                ModifiedAt = File.GetLastWriteTimeUtc(filePath),
                Size = new FileInfo(filePath).Length,
                LineCount = 3
            };

            // Act
            var errors = logFile.LoadEventsFromFile(filePath);

            // Assert
            Assert.Equal(2, logFile.Events.Count); // 2 valid events
            Assert.Single(errors);                // 1 invalid line
            Assert.All(logFile.Events, e => Assert.Equal("Jump", e.GetProperty("event").GetString()));

            // Cleanup
            File.Delete(filePath);
            Directory.Delete(tempDir);
        }
    }
}
