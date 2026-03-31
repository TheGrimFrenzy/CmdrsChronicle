using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace CmdrsChronicle.Core.Tests
{
    public class JournalFileDiscoveryTests
    {
        [Fact]
        public void Only_Journal_Files_Are_Discovered()
        {
            // Arrange: Create a temp directory with test files
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            var filesToCreate = new[]
            {
                "Journal.2026-03-21T193456.01.log",
                "Journal.2026-03-21T193456.02.log",
                "Status.json",
                "Market.json",
                "Journal.2026-03-21T193456.01.txt",
                "Journal.2026-03-21T19.log",
                "Journal.2026-03-21T193456.01.log.bak"
            };
            foreach (var file in filesToCreate)
            {
                File.WriteAllText(Path.Combine(tempDir, file), "test");
            }

            try
            {
                // Act: Use the real discovery logic
                var journalFiles = CmdrsChronicle.Core.JournalFileDiscovery.DiscoverJournalFiles(tempDir);

                // Assert: Only valid journal log files are selected
                Assert.Equal(2, journalFiles.Count);
                Assert.Contains(journalFiles, f => f.EndsWith("Journal.2026-03-21T193456.01.log"));
                Assert.Contains(journalFiles, f => f.EndsWith("Journal.2026-03-21T193456.02.log"));
            }
            finally
            {
                // Cleanup
                foreach (var file in filesToCreate)
                {
                    var path = Path.Combine(tempDir, file);
                    if (File.Exists(path)) File.Delete(path);
                }
                Directory.Delete(tempDir);
            }
        }
    }
}
