using System;
using System.IO;
using Xunit;
using CmdrsChronicle.Core;

namespace CmdrsChronicle.Core.Tests
{
    public class FallbackHelpersTests
    {
        [Fact]
        public void FindMostRecentStarSystem_PicksLatestFileAndParsesEvent()
        {
            var temp = Path.Combine(Path.GetTempPath(), "cc-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(temp);
            try
            {
                // Older file
                var f1 = Path.Combine(temp, "Journal.2026-03-03T120000.00.log");
                File.WriteAllLines(f1, new[] {
                    "{\"timestamp\":\"2026-03-03T10:00:00Z\",\"event\":\"FSDJump\",\"StarSystem\":\"OldSys\",\"Commander\":\"OldCmdr\"}",
                    "{\"timestamp\":\"2026-03-03T10:30:00Z\",\"event\":\"Scan\"}"
                });

                // Newer file (should be chosen)
                var f2 = Path.Combine(temp, "Journal.2026-03-04T120000.00.log");
                File.WriteAllLines(f2, new[] {
                    "{\"timestamp\":\"2026-03-04T08:00:00Z\",\"event\":\"FSDJump\",\"StarSystem\":\"BetaSys\",\"Commander\":\"BetaCmdr\"}",
                    "{\"timestamp\":\"2026-03-04T22:10:00Z\",\"event\":\"FSDJump\",\"StarSystem\":\"Duamta\",\"Commander\":\"CmdrTest\"}"
                });

                var boundary = new DateTime(2026, 3, 5);
                var (lastSystem, lastDate, cmdrName) = ReportHelpers.FindMostRecentStarSystem(temp, boundary);

                Assert.Equal("Duamta", lastSystem);
                Assert.Equal("CmdrTest", cmdrName);
                Assert.Equal(ReportHelpers.FormatLoreDate(DateTime.Parse("2026-03-04T22:10:00Z")), lastDate);
            }
            finally
            {
                try { Directory.Delete(temp, true); } catch { }
            }
        }

        [Fact]
        public void FormatLoreDate_Adds1286Years()
        {
            var d = new DateTime(2026, 3, 5);
            var s = ReportHelpers.FormatLoreDate(d);
            Assert.Equal("3312-03-05", s);
        }
    }
}
