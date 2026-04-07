using System;
using System.IO;
using CmdrsChronicle.Core;
using Xunit;

namespace CmdrsChronicle.Core.Tests
{
    public class JournalFileParallelParsingTests
    {
        [Fact]
        public void Parses_Files_In_Parallel_With_Concurrency_Cap_And_Logs_Errors()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            try
            {
                var validLine   = "{\"event\":\"Jump\",\"timestamp\":\"2026-03-21T19:34:56Z\"}";
                var invalidLine = "not a json line";
                File.WriteAllLines(Path.Combine(tempDir, "Journal.2026-03-21T193456.01.log"), new[] { validLine, invalidLine, validLine });
                File.WriteAllLines(Path.Combine(tempDir, "Journal.2026-03-21T193457.01.log"), new[] { invalidLine, validLine });

                var (events, errors) = JournalFileDiscovery.ParseJournalFilesParallel(tempDir, 2);

                Assert.Equal(3, events.Count); // 3 valid JSON lines across both files
                Assert.Equal(2, errors.Count); // 2 invalid JSON lines
            }
            finally
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }
}
