using CmdrsChronicle.Core;
using Xunit;

namespace CmdrsChronicle.Core.Tests
{
    public class ReservedWordsTests
    {
        [Theory]
        [InlineData("timestamp", "event_timestamp")]
        [InlineData("order", "event_order")]
        [InlineData("group", "event_group")]
        [InlineData("foo", "foo")]
        [InlineData("select", "event_select")]
        [InlineData("JOIN", "event_JOIN")]
        public void PrefixIfReserved_WorksAsExpected(string input, string expected)
        {
            var result = ReservedWords.PrefixIfReserved(input);
            Assert.Equal(expected, result);
        }
    }
}
