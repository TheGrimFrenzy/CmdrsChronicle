using System;
using System.Collections.Generic;
using CmdrsChronicle.Core;
using Xunit;

namespace CmdrsChronicle.Core.Tests
{
    public class ReportTests
    {
        [Fact]
        public void Properties_AreMappedCorrectly()
        {
            var now = DateTime.UtcNow;
            var sections = new List<ReportSection>();
            var assets = new List<string> { "asset1", "asset2" };
            var report = new Report
            {
                Title = "Test Report",
                DateGenerated = now,
                Style = "elegant",
                Sections = sections,
                EmbeddedAssets = assets
            };

            Assert.Equal("Test Report", report.Title);
            Assert.Equal(now, report.DateGenerated);
            Assert.Equal("elegant", report.Style);
            Assert.Equal(sections, report.Sections);
            Assert.Equal(assets, report.EmbeddedAssets);
        }
    }
}
