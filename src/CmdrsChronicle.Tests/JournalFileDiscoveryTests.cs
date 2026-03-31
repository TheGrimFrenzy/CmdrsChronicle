using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using CmdrsChronicle.Core;

namespace CmdrsChronicle.Tests
{
    public class JournalFileDiscoveryTests
    {
        [Fact]
        public void DiscoverJournalFiles_FindsOnlyCanonicalFiles()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            try
            {
                // Canonical files
                var valid1 = Path.Combine(tempDir, "Journal.2026-03-21T193456.01.log");
                var valid2 = Path.Combine(tempDir, "Journal.2026-03-22T101010.02.log");
                File.WriteAllText(valid1, "{}");
                File.WriteAllText(valid2, "{}");
                // Non-canonical files
                File.WriteAllText(Path.Combine(tempDir, "Status.json"), "{}");
                File.WriteAllText(Path.Combine(tempDir, "Journal-2026-03-21.log"), "{}");
                File.WriteAllText(Path.Combine(tempDir, "Journal.2026-03-21T193456.01.txt"), "{}");

                var files = JournalFileDiscovery.DiscoverJournalFiles(tempDir);
                Assert.Contains(valid1, files);
                Assert.Contains(valid2, files);
                Assert.Equal(2, files.Count);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public void ParseJournalFilesParallel_ParsesValidJsonAndLogsErrors()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            try
            {
                var file1 = Path.Combine(tempDir, "Journal.2026-03-21T193456.01.log");
                var file2 = Path.Combine(tempDir, "Journal.2026-03-22T101010.02.log");
                File.WriteAllLines(file1, new[] { "{\"event\":\"Foo\"}", "not json" });
                File.WriteAllLines(file2, new[] { "{\"event\":\"Bar\"}" });

                var (events, errors) = JournalFileDiscovery.ParseJournalFilesParallel(tempDir, 2);
                Assert.Equal(2, events.Count); // Only valid JSON lines
                Assert.Single(errors); // One invalid line
                Assert.Contains("Invalid JSON in Journal.2026-03-21T193456.01.log", errors[0]);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public void DiscoverJournalFiles_ThrowsIfDirectoryMissing()
        {
            var missingDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var ex = Assert.Throws<DirectoryNotFoundException>(() => JournalFileDiscovery.DiscoverJournalFiles(missingDir));
            Assert.Contains("Directory not found", ex.Message);
        }
    }
}
