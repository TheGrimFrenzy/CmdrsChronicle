using System.Text.Json.Serialization;

namespace CmdrsChronicle.Core
{
    /// <summary>
    /// Represents a single "nothing to report" message entry from no-data-messages.json.
    /// Selected by ordinal day of the year when no qualifying journal events are found.
    /// </summary>
    public class NoDataMessage
    {
        /// <summary>GalNet article headline. Maps to the {GALNET_HEADLINE} template placeholder.</summary>
        [JsonPropertyName("title")]
        public string Title { get; set; } = "";

        /// <summary>Ordinal day of the year (1-365) this message is keyed to.</summary>
        [JsonPropertyName("OrdinalDay")]
        public int OrdinalDay { get; set; }

        /// <summary>Optional real-world holiday or observance associated with this day.</summary>
        [JsonPropertyName("holiday")]
        public string? Holiday { get; set; }

        /// <summary>
        /// GalNet article subheading. Maps to the {GALNET_SUBHEAD} template placeholder.
        /// </summary>
        [JsonPropertyName("summary")]
        public string Summary { get; set; } = "";

        /// <summary>
        /// GalNet article body text. Maps to the {GALNET_BODY} template placeholder.
        /// Supports {cmdrName}, {lastSystem}, and {lastDate} interpolation tokens.
        /// </summary>
        [JsonPropertyName("body")]
        public string Body { get; set; } = "";

        /// <summary>
        /// Closing note rendered after the body. Maps to the {GALNET_CLOSING_NOTE} placeholder.
        /// Supports {cmdrName}, {lastSystem}, and {lastDate} interpolation tokens.
        /// </summary>
        [JsonPropertyName("closingNote")]
        public string ClosingNote { get; set; } = "";
    }
}
