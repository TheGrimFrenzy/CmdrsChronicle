using System;
using System.Collections.Generic;
using CmdrsChronicle.Core;
using Xunit;

namespace CmdrsChronicle.Core.Tests
{
    public class NoDataMessageSelectorTests
    {
        // ── LoadMessages ────────────────────────────────────────────────────────

        [Fact]
        public void LoadMessages_ReturnsEmpty_WhenFileNotFound()
        {
            var result = NoDataMessageSelector.LoadMessages("nonexistent-path.json");
            Assert.Empty(result);
        }

        // ── ComputeOrdinalDay ────────────────────────────────────────────────────

        [Theory]
        [InlineData(2025, 1, 1, 1)]    // Jan 1 non-leap → day 1
        [InlineData(2025, 12, 31, 365)] // Dec 31 non-leap → day 365
        [InlineData(2025, 3, 1, 60)]   // Mar 1 non-leap → day 60
        [InlineData(2025, 3, 2, 61)]   // Mar 2 non-leap → day 61
        public void ComputeOrdinalDay_NonLeapYear_ReturnsExpected(int year, int month, int day, int expected)
        {
            var date = new DateTime(year, month, day);
            Assert.Equal(expected, NoDataMessageSelector.ComputeOrdinalDay(date));
        }

        [Theory]
        [InlineData(2024, 1, 1, 1)]    // Jan 1 leap → day 1
        [InlineData(2024, 2, 28, 59)]  // Feb 28 leap → day 59
        [InlineData(2024, 2, 29, 60)]  // Feb 29 leap → day 60 (no shift; it IS day 60)
        [InlineData(2024, 3, 1, 60)]   // Mar 1 leap → DayOfYear=61, shifted to 60
        [InlineData(2024, 3, 2, 61)]   // Mar 2 leap → DayOfYear=62, shifted to 61
        [InlineData(2024, 12, 31, 365)] // Dec 31 leap → DayOfYear=366, shifted to 365
        public void ComputeOrdinalDay_LeapYear_ShiftsAfterFeb29(int year, int month, int day, int expected)
        {
            var date = new DateTime(year, month, day);
            Assert.Equal(expected, NoDataMessageSelector.ComputeOrdinalDay(date));
        }

        // ── SelectByDate ─────────────────────────────────────────────────────────

        [Fact]
        public void SelectByDate_ReturnsNull_WhenListEmpty()
        {
            var result = NoDataMessageSelector.SelectByDate(new List<NoDataMessage>(), new DateTime(2025, 3, 25));
            Assert.Null(result);
        }

        [Fact]
        public void SelectByDate_ReturnsMatchingEntry_ForOrdinalDay()
        {
            var messages = new List<NoDataMessage>
            {
                new() { OrdinalDay = 84, Title = "Day 84 Message", Summary = "s", Body = "b", ClosingNote = "c" },
                new() { OrdinalDay = 10, Title = "Day 10 Message", Summary = "s", Body = "b", ClosingNote = "c" }
            };

            // March 25 non-leap = day 84
            var result = NoDataMessageSelector.SelectByDate(messages, new DateTime(2025, 3, 25), new Random(0));
            Assert.NotNull(result);
            Assert.Equal("Day 84 Message", result!.Title);
        }

        [Fact]
        public void SelectByDate_FallsBackToRandom_WhenNoMatch()
        {
            var messages = new List<NoDataMessage>
            {
                new() { OrdinalDay = 999, Title = "Only Entry", Summary = "s", Body = "b", ClosingNote = "c" }
            };

            // Day 84 has no entry; should fallback to the one entry available
            var result = NoDataMessageSelector.SelectByDate(messages, new DateTime(2025, 3, 25), new Random(0));
            Assert.NotNull(result);
            Assert.Equal("Only Entry", result!.Title);
        }

        [Fact]
        public void SelectByDate_SelectsFromMultipleCandidates_Randomly()
        {
            var messages = new List<NoDataMessage>
            {
                new() { OrdinalDay = 84, Title = "Alpha", Summary = "s", Body = "b", ClosingNote = "c" },
                new() { OrdinalDay = 84, Title = "Beta",  Summary = "s", Body = "b", ClosingNote = "c" }
            };

            var seen = new HashSet<string>();
            for (int i = 0; i < 50; i++)
            {
                var result = NoDataMessageSelector.SelectByDate(messages, new DateTime(2025, 3, 25), new Random(i));
                Assert.NotNull(result);
                seen.Add(result!.Title);
            }
            // Both candidates should appear across 50 different seeds
            Assert.Contains("Alpha", seen);
            Assert.Contains("Beta", seen);
        }

        // ── Interpolate ──────────────────────────────────────────────────────────

        [Fact]
        public void Interpolate_ReplacesAllTokens()
        {
            var text = "Hello {cmdrName}, last seen in {lastSystem} on {lastDate}.";
            var result = NoDataMessageSelector.Interpolate(text, "GrimFrenzy", "Sol", "3312-03-25");
            Assert.Equal("Hello GrimFrenzy, last seen in Sol on 3312-03-25.", result);
        }

        [Fact]
        public void Interpolate_LeavesUnrecognisedTokensUnchanged()
        {
            var text = "Hello {unknown}.";
            var result = NoDataMessageSelector.Interpolate(text, "GrimFrenzy", "Sol", "3312-03-25");
            Assert.Equal("Hello {unknown}.", result);
        }

        [Fact]
        public void Interpolate_HandlesTextWithNoTokens()
        {
            var text = "No tokens here.";
            var result = NoDataMessageSelector.Interpolate(text, "GrimFrenzy", "Sol", "3312-03-25");
            Assert.Equal("No tokens here.", result);
        }
    }
}
