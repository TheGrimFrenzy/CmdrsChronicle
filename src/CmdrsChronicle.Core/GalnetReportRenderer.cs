using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CmdrsChronicle.Core
{
    /// <summary>
    /// Renders the galnet-style (2-column panel grid) HTML report and nothing-to-report page.
    /// Panels use the Elite amber dark theme defined in galnet.css.
    /// </summary>
    public static class GalnetReportRenderer
    {
        private static readonly Random _rng = new();

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
                .Replace("{CSS}",                 css)
                .Replace("{TAGLINE}",             tagline)
                .Replace("{CMDR_NAME}",           cmdrName)
                .Replace("{DATE_FROM}",           dateFrom)
                .Replace("{DATE_TO}",             dateTo)
                .Replace("{GALNET_HEADLINE}",     message.Title)
                .Replace("{GALNET_SUBHEAD}",      message.Summary)
                .Replace("{GALNET_BODY}",         message.Body)
                .Replace("{GALNET_CLOSING_NOTE}", message.ClosingNote);
        }

        // ── Panel rendering ──────────────────────────────────────────────────────

        private static string RenderAllSections(IReadOnlyList<SystemVisit> sections)
        {
            var hasAny = sections.Any(s => s.Results.Any(r => r.MeetsThreshold));
            if (!hasAny)
                return "      <article class=\"panel\" style=\"grid-column:1/-1;\"><div class=\"panel-body\"><p style=\"color:var(--text-muted);\">No qualifying activity recorded for this period.</p></div></article>\n";

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
                        sb.Append("      <p class=\"sys-empty\" style=\"grid-column:1/-1;\">Nothing of note recorded in this system.</p>\n");
                    continue;
                }

                sb.Append(RenderPanels(qualifying));
            }
            return sb.ToString();
        }

        private static string RenderSystemDivider(SystemVisit section)
        {
            var nameHtml = TileRenderer.HtmlEncode(section.SystemName!);
            var dateHtml = section.ArrivalLore != null
                ? $"<span class=\"sys-div-date\" title=\"{TileRenderer.HtmlEncode(section.ArrivalActual ?? string.Empty)}\">{TileRenderer.HtmlEncode(section.ArrivalLore)}</span>"
                : string.Empty;
            return $"      <div class=\"sys-div\" style=\"grid-column:1/-1;\"><span class=\"sys-div-name\">{nameHtml}</span>{dateHtml}</div>\n";
        }

        private static string RenderPanels(IReadOnlyList<InfographicQueryResult> results)
        {
            var sb = new StringBuilder();
            foreach (var r in results)
                sb.Append(RenderPanel(r));
            return sb.ToString();
        }

        private static string RenderPanel(InfographicQueryResult result)
        {
            var def   = result.Definition;
            var count = TileRenderer.HtmlEncode(TileRenderer.FormatMetric(TileRenderer.PrimaryValue(result)));

            var subtitleHtml = !string.IsNullOrWhiteSpace(def.Subtitle)
                ? $"\n          <div class=\"panel-subtitle\">{TileRenderer.HtmlEncode(def.Subtitle)}</div>"
                : string.Empty;

            var body    = RenderPanelBody(result);
            var caption = RenderCaption(def);

            return
$@"      <article class=""panel"">
        <header class=""panel-header"">
          <div>
            <div class=""panel-title"">{TileRenderer.HtmlEncode(def.Title)}</div>{subtitleHtml}
          </div>
          <div class=""panel-count"">{count}</div>
        </header>
        <div class=""panel-body"">
{body}{caption}
        </div>
      </article>
";
        }

        private static string RenderPanelBody(InfographicQueryResult result)
        {
            var def = result.Definition;

            if (string.Equals(def.ChartType, "summary-tile", StringComparison.OrdinalIgnoreCase))
            {
                var text = TileRenderer.SubstitutePlaceholders(def.Details?.Text ?? string.Empty, result);
                return $"          <p>{TileRenderer.HtmlEncode(text)}</p>";
            }

            if (string.Equals(def.ChartType, "table", StringComparison.OrdinalIgnoreCase))
                return RenderPanelTable(result);

            return RenderBarList(result);
        }

        // ── Bar list ─────────────────────────────────────────────────────────────

        private static string RenderBarList(InfographicQueryResult result)
        {
            if (result.DetailRows.Count == 0) return string.Empty;

            var rows        = result.DetailRows;
            var displayRows = new List<(string Label, long Value, bool IsCatchAll)>();

            if (rows.Count <= TileRenderer.MaxDetailRows)
            {
                foreach (var row in rows)
                {
                    var label = row.Length > 0 ? row[0] : "(unknown)";
                    var value = row.Length > 1 && long.TryParse(row[1], out var v) ? v : 0L;
                    displayRows.Add((label, value, false));
                }
            }
            else
            {
                for (int i = 0; i < TileRenderer.MaxDetailRows; i++)
                {
                    var row = rows[i];
                    var label = row.Length > 0 ? row[0] : "(unknown)";
                    var value = row.Length > 1 && long.TryParse(row[1], out var v) ? v : 0L;
                    displayRows.Add((label, value, false));
                }

                var otherLabels = new List<string>();
                long otherTotal = 0;
                for (int i = TileRenderer.MaxDetailRows; i < rows.Count; i++)
                {
                    var row = rows[i];
                    otherLabels.Add(row.Length > 0 ? row[0] : "(unknown)");
                    if (row.Length > 1 && long.TryParse(row[1], out var v))
                        otherTotal += v;
                }
                if (otherTotal > 0)
                    displayRows.Add(($"(+{otherLabels.Count} more)", otherTotal, true));
            }

            var max = displayRows.Max(r => r.Value);
            if (max <= 0) return string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine("          <div class=\"bar-list\">");
            foreach (var (label, value, isCatchAll) in displayRows)
            {
                var pct       = (int)Math.Round(value * 100.0 / max);
                var fillClass = isCatchAll ? "bar-fill bar-fill--muted" : "bar-fill";
                sb.AppendLine("            <div class=\"bar-row\">");
                sb.AppendLine($"              <div class=\"bar-label\"><span>{TileRenderer.HtmlEncode(TileRenderer.FormatLabel(label))}</span><span>{value}</span></div>");
                sb.AppendLine($"              <div class=\"bar-track\"><div class=\"{fillClass}\" style=\"--value: {pct}%;\"></div></div>");
                sb.AppendLine("            </div>");
            }
            sb.Append("          </div>");
            return sb.ToString();
        }

        // ── Panel table ───────────────────────────────────────────────────────────

        private static string RenderPanelTable(InfographicQueryResult result)
        {
            if (result.DetailRows.Count == 0) return string.Empty;

            var def       = result.Definition;
            var colLabel  = def.TableColumns?.Length >= 1 ? def.TableColumns[0] : "Label";
            var colValue  = def.TableColumns?.Length >= 2 ? def.TableColumns[1] : "Value";
            var inaraBase = def.InaraSearchBase;

            var rows        = result.DetailRows;
            var displayRows = new List<(string Label, long Value)>();
            var otherLabels = new List<string>();
            long otherTotal = 0;

            if (rows.Count <= TileRenderer.MaxDetailRows)
            {
                foreach (var row in rows)
                {
                    var label = row.Length > 0 ? row[0] : "(unknown)";
                    var value = row.Length > 1 && long.TryParse(row[1], out var v) ? v : 0L;
                    displayRows.Add((label, value));
                }
            }
            else
            {
                for (int i = 0; i < TileRenderer.MaxDetailRows; i++)
                {
                    var row = rows[i];
                    var label = row.Length > 0 ? row[0] : "(unknown)";
                    var value = row.Length > 1 && long.TryParse(row[1], out var v) ? v : 0L;
                    displayRows.Add((label, value));
                }
                for (int i = TileRenderer.MaxDetailRows; i < rows.Count; i++)
                {
                    var row = rows[i];
                    otherLabels.Add(row.Length > 0 ? row[0] : "(unknown)");
                    if (row.Length > 1 && long.TryParse(row[1], out var v))
                        otherTotal += v;
                }
                displayRows.Add(("Other", otherTotal));
            }

            var sb = new StringBuilder();
            sb.AppendLine("          <table class=\"panel-table\">");
            sb.AppendLine($"            <thead><tr><th>{TileRenderer.HtmlEncode(colLabel)}</th><th>{TileRenderer.HtmlEncode(colValue)}</th></tr></thead>");
            sb.AppendLine("            <tbody>");
            foreach (var (label, value) in displayRows)
            {
                string labelCell;
                if (label == "Other" && otherLabels.Count > 0)
                {
                    var catchAllLabel = $"(+{otherLabels.Count} more)";
                    labelCell = $"<td title=\"{TileRenderer.HtmlEncode(string.Join(", ", otherLabels.Select(TileRenderer.FormatLabel)))}\">{TileRenderer.HtmlEncode(catchAllLabel)}</td>";
                }
                else if (inaraBase != null)
                {
                    var href = TileRenderer.HtmlEncode(inaraBase + Uri.EscapeDataString(label));
                    labelCell = $"<td><a href=\"{href}\" target=\"_blank\" class=\"inara-link\"><span class=\"inara-icon\"></span></a>{TileRenderer.HtmlEncode(TileRenderer.FormatLabel(label))}</td>";
                }
                else
                {
                    labelCell = $"<td>{TileRenderer.HtmlEncode(TileRenderer.FormatLabel(label))}</td>";
                }
                sb.AppendLine($"              <tr>{labelCell}<td>{value}</td></tr>");
            }
            sb.AppendLine("            </tbody>");
            sb.Append("          </table>");
            return sb.ToString();
        }

        // ── Caption ───────────────────────────────────────────────────────────────

        private static string RenderCaption(InfographicDefinition def)
        {
            if (def.TagLines == null || def.TagLines.Length == 0)
                return string.Empty;

            var caption = def.TagLines[_rng.Next(def.TagLines.Length)];
            return $"\n          <div class=\"panel-caption\">{TileRenderer.HtmlEncode(caption)}</div>";
        }
    }
}
