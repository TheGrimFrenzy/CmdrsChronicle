using CmdrsChronicle.Core;
using Xunit;

namespace CmdrsChronicle.Core.Tests
{
    public class InfographicDefinitionTests
    {
        [Fact]
        public void Properties_AreMappedCorrectly()
        {
            var def = new InfographicDefinition
            {
                Category    = "Mining",
                Title       = "Minerals Refined",
                Query       = "SELECT COUNT(*) FROM MiningRefined",
                Threshold   = 3,
                DetailQuery = "SELECT Type, COUNT(*) FROM MiningRefined GROUP BY Type",
                ChartType   = "bar-chart",
                Enabled     = true
            };

            Assert.Equal("Mining",                                       def.Category);
            Assert.Equal("Minerals Refined",                             def.Title);
            Assert.Equal("SELECT COUNT(*) FROM MiningRefined",           def.Query);
            Assert.Equal(3,                                              def.Threshold);
            Assert.Equal("SELECT Type, COUNT(*) FROM MiningRefined GROUP BY Type", def.DetailQuery);
            Assert.Equal("bar-chart",                                    def.ChartType);
            Assert.True(def.Enabled);
        }

        [Fact]
        public void Defaults_AreCorrect()
        {
            var def = new InfographicDefinition();
            Assert.Equal(1,    def.Threshold);
            Assert.True(def.Enabled);
            Assert.Null(def.DetailQuery);
            Assert.Null(def.ChartType);
            Assert.Null(def.SourceFile);
        }
    }
}
