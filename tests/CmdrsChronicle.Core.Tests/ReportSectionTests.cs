using System.Collections.Generic;
using CmdrsChronicle.Core;
using Xunit;

namespace CmdrsChronicle.Core.Tests
{
    public class ReportSectionTests
    {
        [Fact]
        public void Properties_AreMappedCorrectly()
        {
            var events = new List<LogEvent>();
            var infographics = new List<InfographicDefinition>();
            var section = new ReportSection
            {
                Title = "Section 1",
                Content = "<p>Content</p>",
                Events = events,
                Infographics = infographics
            };

            Assert.Equal("Section 1", section.Title);
            Assert.Equal("<p>Content</p>", section.Content);
            Assert.Equal(events, section.Events);
            Assert.Equal(infographics, section.Infographics);
        }
    }
}
