using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CmdrsChronicle.Core
{
    /// <summary>
    /// Selects and interpolates "nothing to report" messages from no-data-messages.json.
    /// Messages are keyed by ordinal day (1-365). Leap years are adjusted so that days
    /// after Feb 29 map back to the same 1-365 index (Mar 1 in a leap year → day 60,
    /// same as Feb 29; Dec 31 in a leap year → day 365).
    /// </summary>
    public static class NoDataMessageSelector
    {
        private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };
        /// <summary>
        /// Loads all messages from the specified JSON file.
        /// Returns an empty list if the file does not exist.
        /// </summary>
        public static List<NoDataMessage> LoadMessages(string jsonPath)
        {
            if (!File.Exists(jsonPath))
                return new List<NoDataMessage>();

            var json = File.ReadAllText(jsonPath);
            return JsonSerializer.Deserialize<List<NoDataMessage>>(json, _jsonOptions)
                ?? new List<NoDataMessage>();
        }

        /// <summary>
        /// Selects a message for the given date using ordinal day lookup.
        /// If multiple entries share the same ordinal day, one is chosen at random.
        /// Falls back to a random message from the full list if no match is found.
        /// Returns null only if the message list is empty.
        /// </summary>
        /// <param name="messages">Loaded message list.</param>
        /// <param name="reportDate">The date to select a message for.</param>
        /// <param name="rng">Optional Random instance for deterministic testing.</param>
        public static NoDataMessage? SelectByDate(List<NoDataMessage> messages, DateTime reportDate, Random? rng = null)
        {
            if (messages.Count == 0)
                return null;

            rng ??= new Random();
            int ordinalDay = ComputeOrdinalDay(reportDate);

            var candidates = messages.Where(m => m.OrdinalDay == ordinalDay).ToList();
            if (candidates.Count > 0)
                return candidates[rng.Next(candidates.Count)];

            // Fallback: random from full list
            return messages[rng.Next(messages.Count)];
        }

        /// <summary>
        /// Computes the 1-365 ordinal day for a given date, with leap year adjustment.
        /// In a leap year, DayOfYear values above 60 (i.e., Mar 1 onward) are decremented
        /// by 1 to re-align with the 1-365 message index (Feb 29 shares day 60 with Mar 1).
        /// </summary>
        public static int ComputeOrdinalDay(DateTime date)
        {
            int day = date.DayOfYear;
            // Feb 29 has DayOfYear == 60 in a leap year. Only days strictly after it are shifted.
            if (DateTime.IsLeapYear(date.Year) && day > 60)
                day--;
            return day;
        }

        /// <summary>
        /// Replaces {cmdrName}, {lastSystem}, and {lastDate} tokens in the given text.
        /// </summary>
        public static string Interpolate(string text, string cmdrName, string lastSystem, string lastDate)
        {
            return text
                .Replace("{cmdrName}", cmdrName)
                .Replace("{lastSystem}", lastSystem)
                .Replace("{lastDate}", lastDate);
        }
    }
}
