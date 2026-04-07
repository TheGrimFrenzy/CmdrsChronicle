using System.Collections.Generic;
using System.Text;

namespace CmdrsChronicle.Core
{
    /// <summary>
    /// Helpers for embedding diagnostic information (parse errors, warnings) into report HTML.
    /// Errors are placed in an HTML comment so they are invisible to end users but inspectable
    /// via View Source, satisfying the "logged in report, not console" requirement (T306).
    /// </summary>
    public static class ReportDiagnostics
    {
        /// <summary>
        /// Formats a list of parse errors as an HTML comment block to append to a report file.
        /// Returns an empty string when <paramref name="errors"/> is null or empty.
        /// </summary>
        public static string FormatParseErrorComment(IReadOnlyList<string> errors)
        {
            if (errors == null || errors.Count == 0)
                return string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("<!--");
            sb.AppendLine($"  CMDRS CHRONICLE — PARSE DIAGNOSTICS ({errors.Count} error(s))");
            sb.AppendLine("  These errors were encountered while reading journal files.");
            sb.AppendLine("  They do not affect tiles that were successfully rendered.");
            sb.AppendLine();
            foreach (var err in errors)
                sb.AppendLine($"  {err}");
            sb.AppendLine("-->");
            return sb.ToString();
        }

        /// <summary>
        /// Returns true when the error list is non-empty, for use in console diagnostics.
        /// </summary>
        public static bool HasErrors(IReadOnlyList<string> errors) =>
            errors != null && errors.Count > 0;
    }
}
