using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CmdrsChronicle.Core
{
    /// <summary>
    /// Renders the elegant-style (grid layout) HTML report and nothing-to-report page.
    /// </summary>
    public static class ElegantReportRenderer
    {
        private static readonly Random _rng = new Random();

        // ── Full report ──────────────────────────────────────────────────────────

        /// <summary>
        /// Renders a full HTML report using the elegant template and CSS.
        /// Placeholders in the template (<c>{TAGLINE}</c>, <c>{TILES}</c>, etc.)
        /// are replaced with live content.
        /// </summary>
        /// <param name="templatePath">Absolute path to the <c>elegant-report.html</c> template file.</param>
        /// <param name="cssPath">Absolute path to the <c>elegant.css</c> stylesheet.</param>
        /// <param name="tagline">Flavour tagline displayed at the top of the report.</param>
        /// <param name="cmdrName">Commander name shown in the report header.</param>
        /// <param name="dateFrom">Start of the reporting period (formatted date string).</param>
        /// <param name="dateTo">End of the reporting period (formatted date string).</param>
        /// <param name="sections">Per-system (or single aggregate) sections containing infographic tiles.</param>
        /// <returns>Self-contained HTML string ready to be written to a file.</returns>
        public static string Render(
            string templatePath,
            string cssPath,
            string tagline,
            string cmdrName,
            string dateFrom,
            string dateTo,
            IReadOnlyList<SystemVisit> sections)
        {
            return TileRenderer.ApplyReportTemplate(
                templatePath, cssPath, tagline, cmdrName, dateFrom, dateTo,
                RenderAllSections(sections));
        }

        // ── Nothing-to-report page ───────────────────────────────────────────────

        /// <summary>
        /// Renders a "nothing to report" HTML page using the elegant nothing-to-report template.
        /// Used when no infographic data exists for the requested date range.
        /// </summary>
        /// <param name="templatePath">Absolute path to the <c>elegant-nothing-to-report.html</c> template file.</param>
        /// <param name="cssPath">Absolute path to the <c>elegant.css</c> stylesheet.</param>
        /// <param name="tagline">Flavour tagline displayed on the page.</param>
        /// <param name="cmdrName">Commander name shown in the page header.</param>
        /// <param name="dateFrom">Start of the reporting period (formatted date string).</param>
        /// <param name="dateTo">End of the reporting period (formatted date string).</param>
        /// <param name="message">Randomly chosen flavour message explaining the lack of data.</param>
        /// <returns>Self-contained HTML string ready to be written to a file.</returns>
        public static string RenderNothingToReport(
            string templatePath,
            string cssPath,
            string tagline,
            string cmdrName,
            string dateFrom,
            string dateTo,
            NoDataMessage message)
        {
            return TileRenderer.ApplyNothingToReportTemplate(
                templatePath, cssPath, tagline, cmdrName, dateFrom, dateTo, message);
        }

        // ── Grid tile rendering ──────────────────────────────────────────────────

        private static string RenderAllSections(IReadOnlyList<SystemVisit> sections)
        {
            var hasAny = sections.Any(s => s.Results.Any(r => r.MeetsThreshold));
            if (!hasAny)
                return "        <p style=\"color:#d4af37;text-align:center;font-family:'Cinzel',serif;padding:40px 0;\">No qualifying activity recorded for this period.</p>";

            var sb = new StringBuilder();
            foreach (var section in sections)
            {
                var qualifying = section.Results
                    .Where(r => r.MeetsThreshold)
                    .ToList();

                if (section.SystemName != null)
                    sb.Append(RenderSystemDivider(section));

                if (qualifying.Count == 0)
                {
                    if (section.SystemName != null)
                        sb.AppendLine("        <p class=\"sys-empty\">Nothing of note recorded in this system.</p>");
                    continue;
                }

                sb.Append(RenderTiles(qualifying));
            }
            return sb.ToString();
        }

        private static string RenderSystemDivider(SystemVisit section)
            => TileRenderer.RenderSystemDivider(section, "        ");

        private static string RenderTiles(IReadOnlyList<InfographicQueryResult> results)
        {
            var sb = new StringBuilder();
            foreach (var result in results)
                sb.Append(RenderTile(result));
            return sb.ToString();
        }

        private static string RenderTile(InfographicQueryResult result)
        {
            var def           = result.Definition;
            var categoryClass = TileRenderer.CategoryClass(def.Category);
            var metricValue   = TileRenderer.FormatMetric(TileRenderer.PrimaryValue(result));
            var bodyHtml      = TileRenderer.RenderTileBody(result);
            var caption       = RenderCaption(def);

            return $@"        <div class='infographic-tile{categoryClass}'>
            <div class='tile-badge'></div>
            <div class='tile-header'>
                <div class='tile-title'>{TileRenderer.HtmlEncode(def.Title)}</div>
                <div class='tile-metric'>
                    <div class='tile-metric-value'>{TileRenderer.HtmlEncode(metricValue)}</div>
                </div>
            </div>
            <div class='tile-body'>
                {bodyHtml}{caption}
            </div>
        </div>
";
        }

        private static string RenderCaption(InfographicDefinition def)
        {
            if (def.TagLines == null || def.TagLines.Length == 0)
                return string.Empty;

            var caption = def.TagLines[_rng.Next(def.TagLines.Length)];
            return $"\n            <div class=\"tile-caption\">{TileRenderer.HtmlEncode(caption)}</div>";
        }
    }
}
