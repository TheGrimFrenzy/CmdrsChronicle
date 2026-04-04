using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CmdrsChronicle.Core
{
    /// <summary>
    /// Renders the elegant-style (grid layout) HTML report and nothing-to-report page.
    /// </summary>
    public static class ElegantReportRenderer
    {
        // ── Full report ──────────────────────────────────────────────────────────

        public static string Render(
            string templatePath,
            string cssPath,
            string tagline,
            string cmdrName,
            string dateFrom,
            string dateTo,
            IReadOnlyList<SystemVisit> sections)
        {
            var template = File.ReadAllText(templatePath);
            var css      = File.ReadAllText(cssPath);

            return template
                .Replace("{CSS}",            css)
                .Replace("{TAGLINE}",        tagline)
                .Replace("{CMDR_NAME}",      cmdrName)
                .Replace("{DATE_FROM}",      dateFrom)
                .Replace("{DATE_TO}",        dateTo)
                .Replace("{DATE_GENERATED}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm") + " UTC")
                .Replace("{TILES}",          RenderAllSections(sections));
        }

        // ── Nothing-to-report page ───────────────────────────────────────────────

        public static string RenderNothingToReport(
            string templatePath,
            string cssPath,
            string tagline,
            string cmdrName,
            string dateFrom,
            string dateTo,
            NoDataMessage message)
        {
            var template = File.ReadAllText(templatePath);
            var css      = File.ReadAllText(cssPath);

            return template
                .Replace("{CSS}",                css)
                .Replace("{TAGLINE}",            tagline)
                .Replace("{CMDR_NAME}",          cmdrName)
                .Replace("{DATE_FROM}",          dateFrom)
                .Replace("{DATE_TO}",            dateTo)
                .Replace("{GALNET_HEADLINE}",    message.Title)
                .Replace("{GALNET_SUBHEAD}",     message.Summary)
                .Replace("{GALNET_BODY}",        message.Body)
                .Replace("{GALNET_CLOSING_NOTE}", message.ClosingNote);
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
        {
            var nameHtml = TileRenderer.HtmlEncode(section.SystemName!);
            var dateHtml = section.ArrivalLore != null
                ? $"<span class=\"sys-div-date\" title=\"{TileRenderer.HtmlEncode(section.ArrivalActual ?? string.Empty)}\">{TileRenderer.HtmlEncode(section.ArrivalLore)}</span>"
                : string.Empty;
            return $"        <div class=\"sys-div\"><span class=\"sys-div-name\">{nameHtml}</span>{dateHtml}</div>\n";
        }

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
            var metricLabel   = string.Equals(def.ChartType, "table", StringComparison.OrdinalIgnoreCase) ? "Total" : string.Empty;
            var bodyHtml      = TileRenderer.RenderTileBody(result);

            return $@"        <div class='infographic-tile{categoryClass}'>
            <div class='tile-badge'></div>
            <div class='tile-header'>
                <div class='tile-title'>{TileRenderer.HtmlEncode(def.Title)}</div>
                <div class='tile-metric'>
                    <div class='tile-metric-value'>{TileRenderer.HtmlEncode(metricValue)}</div>
                    <div class='tile-metric-label'>{TileRenderer.HtmlEncode(metricLabel)}</div>
                </div>
            </div>
            <div class='tile-body'>
                {bodyHtml}
            </div>
        </div>
";
        }
    }
}
