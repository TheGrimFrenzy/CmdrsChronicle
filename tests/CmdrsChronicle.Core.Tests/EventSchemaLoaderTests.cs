using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using CmdrsChronicle.Core;
using Xunit;

namespace CmdrsChronicle.Core.Tests
{
    public class EventSchemaLoaderTests
    {
        [Fact]
        public void LoadAllSchemas_ThrowsIfDirectoryMissing()
        {
            Assert.Throws<DirectoryNotFoundException>(() =>
                EventSchemaLoader.LoadAllSchemas("nonexistent_dir"));
        }

        [Fact]
        public void LoadAllSchemas_LoadsAllJsonFiles()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            var file1 = Path.Combine(tempDir, "TestEvent1.json");
            var file2 = Path.Combine(tempDir, "TestEvent2.json");
            File.WriteAllText(file1, "{\"title\":\"Test1\"}");
            File.WriteAllText(file2, "{\"title\":\"Test2\"}");

            try
            {
                var result = EventSchemaLoader.LoadAllSchemas(tempDir);
                Assert.Equal(2, result.Count);
                Assert.Contains("TestEvent1", result.Keys);
                Assert.Contains("TestEvent2", result.Keys);
                Assert.Equal("Test1", result["TestEvent1"].RootElement.GetProperty("title").GetString());
                Assert.Equal("Test2", result["TestEvent2"].RootElement.GetProperty("title").GetString());
            }
            finally
            {
                File.Delete(file1);
                File.Delete(file2);
                Directory.Delete(tempDir);
            }
        }
    }
}
