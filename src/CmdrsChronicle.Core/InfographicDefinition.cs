using System.Text.Json.Serialization;

namespace CmdrsChronicle.Core
{
    /// <summary>
    /// Represents an infographic tile definition loaded from a JSON file in /infographics/{Category}/.
    /// Properties are deserialized case-insensitively to match the camelCase JSON format.
    /// </summary>
    public class InfographicDefinition
    {
        /// <summary>Category for grouping (e.g. "Mining", "Exploration").</summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>Display title for the infographic tile.</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// SQL query to compute the main metric value.
        /// Must return a single scalar (e.g. COUNT(*) AS count).
        /// Supports :startDate and :endDate parameters.
        /// </summary>
        public string Query { get; set; } = string.Empty;

        /// <summary>Minimum value required to include this tile in the report. Default: 1.</summary>
        public int Threshold { get; set; } = 1;

        /// <summary>
        /// SQL query for breakdown/chart data.
        /// Must return (label TEXT, value INTEGER) rows.
        /// Supports :startDate and :endDate parameters.
        /// </summary>
        public string? DetailQuery { get; set; }

        /// <summary>Visualization type for the detail query (e.g. "bar-chart").</summary>
        public string? ChartType { get; set; }

        /// <summary>If false, this tile is excluded during loading. Default: true.</summary>
        public bool Enabled { get; set; } = true;

        /// <summary>Optional descriptive details used for summary tiles. Not required.</summary>
        public InfographicDetails? Details { get; set; }

        /// <summary>
        /// Column header labels for table chart type. Two elements: label column header, value column header.
        /// Defaults to ["Label", "Value"] when absent.
        /// </summary>
        public string[]? TableColumns { get; set; }

        /// <summary>
        /// Base URL for Inara.cz search links used in table rows, e.g. "https://inara.cz/elite/station/?search=".
        /// The URL-encoded row label is appended. When null, no links are rendered.
        /// </summary>
        public string? InaraSearchBase { get; set; }

        /// <summary>Optional subtitle shown beneath the panel title in galnet style (e.g. "Genera recorded").</summary>
        public string? Subtitle { get; set; }

        /// <summary>
        /// Optional list of flavour captions for the galnet panel-caption area.
        /// One is chosen at random at render time. If absent, no caption is rendered.
        /// </summary>
        public string[]? Captions { get; set; }

        /// <summary>Source file path set by InfographicLoader; not read from JSON.</summary>
        [JsonIgnore]
        public string? SourceFile { get; set; }
    }

    public class InfographicDetails
    {
        /// <summary>Descriptive text supporting placeholders like {count} or {totalReward}.</summary>
        public string? Text { get; set; }

        /// <summary>Optional help/tooltip text to show for the tile.</summary>
        public string? Help { get; set; }
    }
}
