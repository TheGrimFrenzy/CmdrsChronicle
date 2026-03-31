using System.IO;
using CmdrsChronicle.Core;
using Xunit;

namespace CmdrsChronicle.Core.Tests
{
    public class InfographicLoaderTests
    {
        [Fact]
        public void LoadAll_ReturnsEmpty_WhenDirectoryDoesNotExist()
        {
            var result = InfographicLoader.LoadAll(Path.Combine(Path.GetTempPath(), "cmdrschronicle_nodir_xyz"));
            Assert.Empty(result);
        }

        [Fact]
        public void LoadAll_DeserializesAllProperties()
        {
            var dir = Directory.CreateTempSubdirectory("infographics_test_");
            try
            {
                var sub = Directory.CreateDirectory(Path.Combine(dir.FullName, "Mining"));
                File.WriteAllText(Path.Combine(sub.FullName, "test.json"), """
                    {
                      "category": "Mining",
                      "title": "Test Tile",
                      "query": "SELECT 1",
                      "threshold": 5,
                      "detailQuery": "SELECT 'a', 1",
                      "chartType": "bar-chart",
                      "enabled": true
                    }
                    """);

                var result = InfographicLoader.LoadAll(dir.FullName);

                Assert.Single(result);
                var def = result[0];
                Assert.Equal("Mining",       def.Category);
                Assert.Equal("Test Tile",    def.Title);
                Assert.Equal("SELECT 1",     def.Query);
                Assert.Equal(5,              def.Threshold);
                Assert.Equal("SELECT 'a', 1", def.DetailQuery);
                Assert.Equal("bar-chart",    def.ChartType);
                Assert.True(def.Enabled);
                Assert.Contains("test.json", def.SourceFile);
            }
            finally { dir.Delete(true); }
        }

        [Fact]
        public void LoadAll_SkipsDisabledDefinitions()
        {
            var dir = Directory.CreateTempSubdirectory("infographics_disabled_");
            try
            {
                File.WriteAllText(Path.Combine(dir.FullName, "off.json"),
                    """{ "category": "Test", "title": "Off", "query": "SELECT 1", "enabled": false }""");

                var result = InfographicLoader.LoadAll(dir.FullName);
                Assert.Empty(result);
            }
            finally { dir.Delete(true); }
        }

        [Fact]
        public void LoadAll_SkipsMalformedJson()
        {
            var dir = Directory.CreateTempSubdirectory("infographics_malformed_");
            try
            {
                File.WriteAllText(Path.Combine(dir.FullName, "broken.json"), "not valid json {{{");

                var result = InfographicLoader.LoadAll(dir.FullName);
                Assert.Empty(result);
            }
            finally { dir.Delete(true); }
        }

        [Fact]
        public void LoadAll_LoadsMultipleFilesAcrossSubdirectories()
        {
            var dir = Directory.CreateTempSubdirectory("infographics_multi_");
            try
            {
                var mining = Directory.CreateDirectory(Path.Combine(dir.FullName, "Mining"));
                var exploration = Directory.CreateDirectory(Path.Combine(dir.FullName, "Exploration"));

                File.WriteAllText(Path.Combine(mining.FullName, "a.json"),
                    """{ "category": "Mining", "title": "A", "query": "SELECT 1", "enabled": true }""");
                File.WriteAllText(Path.Combine(exploration.FullName, "b.json"),
                    """{ "category": "Exploration", "title": "B", "query": "SELECT 1", "enabled": true }""");

                var result = InfographicLoader.LoadAll(dir.FullName);
                Assert.Equal(2, result.Count);
            }
            finally { dir.Delete(true); }
        }
    }
}
