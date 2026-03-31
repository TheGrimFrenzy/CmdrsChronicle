using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CmdrsChronicle.Core.Tests
{
    public class JournalFileParallelParsingTests
    {
        [Fact]
        public void Parses_Files_In_Parallel_With_Concurrency_Cap_And_Logs_Errors()
        {
            // Arrange: Create temp directory and files
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            var files = new[]
            {
                "Journal.2026-03-21T193456.01.log",
                "Journal.2026-03-21T193457.01.log"
            };
            var validLine = "{\"event\":\"Jump\",\"timestamp\":\"2026-03-21T19:34:56Z\"}";
            var invalidLine = "not a json line";
            File.WriteAllLines(Path.Combine(tempDir, files[0]), new[] { validLine, invalidLine, validLine });
            File.WriteAllLines(Path.Combine(tempDir, files[1]), new[] { invalidLine, validLine });

            var parsedEvents = new ConcurrentBag<JsonElement>();
            var errorLog = new ConcurrentBag<string>();
            var concurrencyObserved = new ConcurrentBag<int>();
            int maxConcurrency = 2;
            int currentConcurrency = 0;

            // Act: Parse in parallel with concurrency cap
            Parallel.ForEach(
                Directory.EnumerateFiles(tempDir, "Journal.*.log"),
                new ParallelOptions { MaxDegreeOfParallelism = maxConcurrency },
                file =>
                {
                    Interlocked.Increment(ref currentConcurrency);
                    concurrencyObserved.Add(currentConcurrency);
                    foreach (var line in File.ReadLines(file))
                    {
                        try
                        {
                            var doc = JsonDocument.Parse(line);
                            parsedEvents.Add(doc.RootElement.Clone());
                        }
                        catch (JsonException)
                        {
                            errorLog.Add($"Invalid JSON in {Path.GetFileName(file)}: {line}");
                        }
                    }
                    Interlocked.Decrement(ref currentConcurrency);
                });

            // Assert: All valid lines parsed, errors logged, concurrency cap respected
            Assert.Equal(3, parsedEvents.Count); // 3 valid lines
            Assert.Equal(2, errorLog.Count);     // 2 invalid lines
            Assert.True(concurrencyObserved.Max() <= maxConcurrency);

            // Cleanup
            foreach (var file in files)
            {
                var path = Path.Combine(tempDir, file);
                if (File.Exists(path)) File.Delete(path);
            }
            Directory.Delete(tempDir);
        }
    }
}
