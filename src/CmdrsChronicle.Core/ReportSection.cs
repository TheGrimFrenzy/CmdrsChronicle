using System.Collections.Generic;

namespace CmdrsChronicle.Core
{
    /// <summary>
    /// Represents a section of a report, containing events and infographics.
    /// </summary>
    public class ReportSection
    {
        /// <summary>Title of the section.</summary>
        public string Title { get; set; }
        /// <summary>HTML content for the section.</summary>
        public string Content { get; set; }
        /// <summary>Events included in this section.</summary>
        public List<LogEvent> Events { get; set; } = new();
        /// <summary>Infographics included in this section.</summary>
        public List<InfographicDefinition> Infographics { get; set; } = new();
    }
}
