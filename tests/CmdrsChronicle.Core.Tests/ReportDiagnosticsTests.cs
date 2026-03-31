using System.Collections.Generic;
using CmdrsChronicle.Core;
using Xunit;

namespace CmdrsChronicle.Core.Tests
{
    public class ReportDiagnosticsTests
    {
        // ── HasErrors ────────────────────────────────────────────────────────────

        [Fact]
        public void HasErrors_ReturnsFalse_WhenListIsEmpty()
        {
            Assert.False(ReportDiagnostics.HasErrors(new List<string>()));
        }

        [Fact]
        public void HasErrors_ReturnsFalse_WhenListIsNull()
        {
            Assert.False(ReportDiagnostics.HasErrors(null));
        }

        [Fact]
        public void HasErrors_ReturnsTrue_WhenListHasItems()
        {
            Assert.True(ReportDiagnostics.HasErrors(new List<string> { "some error" }));
        }

        // ── FormatParseErrorComment ──────────────────────────────────────────────

        [Fact]
        public void FormatParseErrorComment_ReturnsEmpty_WhenNoErrors()
        {
            var result = ReportDiagnostics.FormatParseErrorComment(new List<string>());
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void FormatParseErrorComment_ReturnsEmpty_WhenNull()
        {
            var result = ReportDiagnostics.FormatParseErrorComment(null);
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void FormatParseErrorComment_ReturnsHtmlComment_WhenErrorsPresent()
        {
            var errors = new List<string> { "bad line in Journal.log", "unexpected token" };

            var result = ReportDiagnostics.FormatParseErrorComment(errors);

            Assert.Contains("<!--", result);
            Assert.Contains("-->", result);
            Assert.Contains("bad line in Journal.log", result);
            Assert.Contains("unexpected token", result);
        }

        [Fact]
        public void FormatParseErrorComment_IncludesErrorCount()
        {
            var errors = new List<string> { "err1", "err2", "err3" };

            var result = ReportDiagnostics.FormatParseErrorComment(errors);

            Assert.Contains("3", result);
        }

        [Fact]
        public void FormatParseErrorComment_DoesNotContainConsoleEscapes()
        {
            var errors = new List<string> { "some error" };

            var result = ReportDiagnostics.FormatParseErrorComment(errors);

            // Should be plain HTML comment, no ANSI escape codes
            Assert.DoesNotContain("\x1b[", result);
        }
    }
}
