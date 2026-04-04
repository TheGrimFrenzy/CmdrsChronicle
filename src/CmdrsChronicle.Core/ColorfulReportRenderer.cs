using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CmdrsChronicle.Core
{
    /// <summary>
    /// Renders the colorful-style (float/timeline layout) HTML report and nothing-to-report page.
    /// Tiles alternate left/right on the timeline so left-facing tiles always occupy the left
    /// column and right-facing tiles always occupy the right column.
    /// </summary>
    public static class ColorfulReportRenderer
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

        // ── Timeline tile rendering ──────────────────────────────────────────────

        private static string RenderAllSections(IReadOnlyList<SystemVisit> sections)
        {
            var hasAny = sections.Any(s => s.Results.Any(r => r.MeetsThreshold));
            if (!hasAny)
                return "        <p style=\"color:#fff;text-align:center;font-family:'Orbitron',sans-serif;padding:40px 0;\">No qualifying activity recorded for this period.</p>";

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

                // Restart left/right alternation at start of each system section
                sb.Append(RenderTilesSection(qualifying, 0));
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

        private static string RenderTilesSection(IReadOnlyList<InfographicQueryResult> results, int startIndex)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < results.Count; i++)
            {
                var absIndex = startIndex + i;
                sb.AppendLine($"        <div class=\"tl-row\" style=\"z-index: {absIndex + 1}\">\n");
                sb.Append(RenderTileFloat(results[i], isLeft: absIndex % 2 == 0));
                sb.AppendLine("        </div>\n");
            }
            return sb.ToString();
        }

        private static string RenderTileFloat(InfographicQueryResult result, bool isLeft)
        {
            var def           = result.Definition;
            var categoryClass = TileRenderer.CategoryClass(def.Category);
            var badgeText     = TileRenderer.HtmlEncode(TileRenderer.FormatMetric(TileRenderer.PrimaryValue(result)));
            var icon          = TileRenderer.CategoryIcon(def.Category);
            var header        = TileRenderer.RenderTileHeader(result, includeMetric: false);
            var body          = TileRenderer.RenderTileBody(result);

            if (isLeft)
            {
                return $@"        <div class='left-tile-wrapper'>
            <div class='left-icon'><span title='{TileRenderer.HtmlEncode(TileRenderer.CategoryDisplayName(def.Category))}'>{icon}</span></div>
            <div class='left-tile{categoryClass}'>
                <div class='left-badge-section'><div class='left-badge'>{badgeText}</div></div>
                <div class='left-content'>
                <div class='tile-flex'>
{header}
                <div class='tile-body-center'>
{body}
                </div>
                </div>
                </div>
            </div>
        </div>
";
            }
            else
            {
                return $@"        <div class='right-tile-wrapper'>
            <div class='right-tile{categoryClass}'>
                <div class='right-content'>
                <div class='tile-flex'>
{header}
                <div class='tile-body-center'>
{body}
                </div>
                </div>
                </div>
                <div class='right-badge-section'><div class='right-badge'>{badgeText}</div></div>
            </div>
            <div class='right-icon'><span title='{TileRenderer.HtmlEncode(TileRenderer.CategoryDisplayName(def.Category))}'>{icon}</span></div>
        </div>
";
            }
        }
    }
}
