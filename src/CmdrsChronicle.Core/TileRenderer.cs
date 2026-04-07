using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CmdrsChronicle.Core
{
    /// <summary>
    /// Shared tile-rendering helpers used by all three report renderers
    /// (Elegant, Colorful, and Galnet). Also owns the HTML template-application
    /// helpers so their identical <c>Render</c> and <c>RenderNothingToReport</c>
    /// bodies live in one place.
    /// </summary>
    internal static class TileRenderer
    {
        internal const int MaxDetailRows = 5;

        // Known CSS gradient class tokens. Stored once as a static field so
        // CategoryClass() never allocates a new array on each call.
        private static readonly string[] _allowedGradients =
        {
            "travel", "combat", "exploration", "exobiology", "trade",
            "powerplay", "fleetcarriers", "colonisation", "onfoot",
            "mining", "engineering", "crew"
        };

        // ── Shared template application ──────────────────────────────────────────
        // All three renderers share the same HTML template structure — only which
        // tiles-HTML string they inject differs. These helpers hold that shared logic.

        /// <summary>
        /// Reads <paramref name="templatePath"/> and <paramref name="cssPath"/>, then
        /// fills in the standard report placeholders and injects the pre-rendered tiles HTML.
        /// Called by every renderer's <c>Render()</c> method.
        /// </summary>
        internal static string ApplyReportTemplate(
            string templatePath, string cssPath,
            string tagline, string cmdrName, string dateFrom, string dateTo,
            string tilesHtml)
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
                .Replace("{TILES}",          tilesHtml);
        }

        /// <summary>
        /// Reads the nothing-to-report template and CSS, then fills in all placeholders
        /// including the GalNet-flavoured message fields.
        /// Called by every renderer's <c>RenderNothingToReport()</c> method.
        /// </summary>
        internal static string ApplyNothingToReportTemplate(
            string templatePath, string cssPath,
            string tagline, string cmdrName, string dateFrom, string dateTo,
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

        /// <summary>
        /// Renders the HTML divider shown between star-system sections in a by-system report.
        /// Each renderer passes its own <paramref name="indent"/> and any extra div attributes
        /// (e.g. Galnet needs <c>style="grid-column:1/-1;"</c>).
        /// </summary>
        internal static string RenderSystemDivider(SystemVisit section, string indent, string extraDivAttributes = "")
        {
            var nameHtml = HtmlEncode(section.SystemName!);
            // If we know when the commander arrived, show a tooltip with the real-world date
            // and display the lore date (real year + 1286) in the header itself.
            var dateHtml = section.ArrivalLore != null
                ? $"<span class=\"sys-div-date\" title=\"{HtmlEncode(section.ArrivalActual ?? string.Empty)}\">{HtmlEncode(section.ArrivalLore)}</span>"
                : string.Empty;
            return $"{indent}<div class=\"sys-div\"{extraDivAttributes}><span class=\"sys-div-name\">{nameHtml}</span>{dateHtml}</div>\n";
        }

        // ── Category slug ────────────────────────────────────────────────────────

        internal static string NormalizeCategory(string category)
        {
            var s = category.ToLowerInvariant();
            s = Regex.Replace(s, "[^a-z0-9]+", "-");
            s = s.Trim('-');
            return s;
        }

        /// <summary>
        /// Maps a raw category string to a CSS gradient class suffix (e.g. "combat").
        /// Falls back to the first slug segment when no known token matches.
        /// </summary>
        internal static string CategoryClass(string? category)
        {
            if (string.IsNullOrWhiteSpace(category)) return string.Empty;

            var slug = NormalizeCategory(category);

            string? chosen = null;
            foreach (var a in _allowedGradients) if (slug == a)            { chosen = a; break; }
            if (chosen == null)
                foreach (var a in _allowedGradients) if (slug.Contains(a)) { chosen = a; break; }
            if (chosen == null)
            {
                var parts = slug.Split('-');
                foreach (var p in parts) if (_allowedGradients.Contains(p)) { chosen = p; break; }
                // Fall back to the first hyphen-segment so at least something is returned.
                if (chosen == null) chosen = parts[0];
            }

            return " grad-" + chosen;
        }

        private static readonly Dictionary<string, string> _categoryDisplayNames = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Exploration",                  "Exploration" },
            { "Missions",                     "Missions" },
            { "CombatShip",                   "Ship Combat" },
            { "CombatOnFoot",                 "On-Foot Combat" },
            { "Trade",                        "Trade" },
            { "Mining",                       "Mining" },
            { "Exobiology",                   "Exobiology" },
            { "Powerplay",                    "Power Play" },
            { "FleetCarrierOperations",       "Fleet Carrier Operations" },
            { "EngineeringAndSynthesis",      "Engineering & Synthesis" },
            { "MaterialGathering",            "Material Gathering" },
            { "PassengerTransport",           "Passenger Transport" },
            { "CrimeAndSecurity",             "Crime & Security" },
            { "SalvageAndRecovery",           "Salvage & Recovery" },
            { "SocialMulticrew",              "Social & Multicrew" },
            { "SettlementActivities",         "Settlement Activities" },
            { "ShipManagementAndOutfitting",  "Ship Management & Outfitting" },
            { "TravelAndNavigation",          "Travel & Navigation" },
            { "ThargoidAX",                   "Thargoid & AX Combat" },
            { "Administrivia",                "Administrivia" },
            { "CodexAndDiscoveries",          "Codex & Discoveries" },
            { "EconomyAndMarket",             "Economy & Market" },
        };

        /// <summary>Returns a human-readable display name for a category (e.g. "CombatShip" → "Ship Combat").</summary>
        internal static string CategoryDisplayName(string? category)
        {
            if (string.IsNullOrWhiteSpace(category)) return string.Empty;
            if (_categoryDisplayNames.TryGetValue(category, out var name)) return name;
            // Fallback: insert spaces at PascalCase boundaries
            return Regex.Replace(category, @"(?<=[a-z])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])", " ");
        }

        /// <summary>Returns an emoji icon character for the given category.</summary>
        internal static string CategoryIcon(string? category)        {
            if (string.IsNullOrWhiteSpace(category)) return "✦";
            var slug = NormalizeCategory(category);
            if (slug.Contains("exobiology"))             return "🧬";
            if (slug.Contains("exploration"))            return "🔭";
            if (slug.Contains("thargoid") || slug.Contains("ax")) return "🛡";
            if (slug.Contains("combat") && slug.Contains("ship")) return "⚔";
            if (slug.Contains("combat") && slug.Contains("foot")) return "🔫";
            if (slug.Contains("crime") || slug.Contains("security")) return "⚖";
            if (slug.Contains("mining"))                 return "⛏";
            if (slug.Contains("engineering"))            return "🔧";
            if (slug.Contains("material"))               return "🧪";
            if (slug.Contains("economy") || slug.Contains("market")) return "💹";
            if (slug.Contains("trade"))                  return "📦";
            if (slug.Contains("mission"))                return "📋";
            if (slug.Contains("passenger"))              return "🧑";
            if (slug.Contains("salvage") || slug.Contains("recovery")) return "🆘";
            if (slug.Contains("settlement") || slug.Contains("foot") || slug.Contains("onfoot")) return "🪖";
            if (slug.Contains("fleet") || slug.Contains("carrier")) return "⚓";
            if (slug.Contains("colonis"))                return "🏗";
            if (slug.Contains("power"))                  return "👑";
            if (slug.Contains("social") || slug.Contains("crew") || slug.Contains("multicrew")) return "👥";
            if (slug.Contains("ship") || slug.Contains("outfitting")) return "🔩";
            if (slug.Contains("travel") || slug.Contains("navigation")) return "🚀";
            if (slug.Contains("codex") || slug.Contains("discover")) return "📖";
            if (slug.Contains("admin") || slug.Contains("administrivia")) return "📊";
            return "✦";
        }

        // ── Primary metric selection ─────────────────────────────────────────────

        internal static long PrimaryValue(InfographicQueryResult result)
        {
            long v = result.MainValue;
            if (result.Scalars != null)
            {
                var rewardKey = result.Scalars.Keys
                    .FirstOrDefault(k => k.IndexOf("reward", StringComparison.OrdinalIgnoreCase) >= 0);
                if (rewardKey != null)                                               v = result.Scalars[rewardKey];
                else if (result.Scalars.ContainsKey("total") && result.Scalars["total"] > 0) v = result.Scalars["total"];
                else if (result.Scalars.ContainsKey("count"))                        v = result.Scalars["count"];
            }
            return v;
        }

        // ── Label formatting ─────────────────────────────────────────────────────

        internal static string FormatLabel(string raw)
        {
            // 1. Strip trailing semicolon (Elite localisation key suffix)
            var s = raw.TrimEnd(';');

            // 2. Split on underscores
            var underscoreParts = s.Split('_');

            // 3. Drop any segment that starts with '$' (Elite key-type prefixes)
            var meaningful = underscoreParts.Where(p => !p.StartsWith("$", StringComparison.Ordinal)).ToList();

            // 4. Fall back to the raw value if stripping left nothing
            if (meaningful.Count == 0)
                return raw;

            // 5. Split each segment on CamelCase boundaries, then title-case each word
            var words = new List<string>();
            foreach (var part in meaningful)
            {
                var split = Regex.Split(part, @"(?<=[a-z])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])");
                foreach (var word in split)
                {
                    if (word.Length == 0) continue;
                    words.Add(char.ToUpperInvariant(word[0]) + word.Substring(1).ToLowerInvariant());
                }
            }

            return string.Join(" ", words);
        }

        // ── Number formatting ────────────────────────────────────────────────────

        internal static string FormatMetric(long value)
        {
            if (value < 1_000) return value.ToString(CultureInfo.CurrentCulture);

            var units = new[] { (Suffix: "B", Div: 1_000_000_000L),
                                (Suffix: "M", Div: 1_000_000L),
                                (Suffix: "K", Div: 1_000L) };
            foreach (var (Suffix, Div) in units)
            {
                if (value >= Div)
                {
                    double scaled = (double)value / Div;
                    string truncated;
                    bool exact;
                    if (scaled >= 10.0)
                    {
                        long floored = (long)Math.Floor(scaled);
                        truncated = floored.ToString("F0", CultureInfo.CurrentCulture);
                        exact = value == floored * Div;
                    }
                    else
                    {
                        double floored1dp = Math.Floor(scaled * 10.0) / 10.0;
                        truncated = floored1dp.ToString("0.#", CultureInfo.CurrentCulture);
                        exact = value == (long)(floored1dp * Div);
                    }
                    return truncated + Suffix;
                }
            }
            return value.ToString(CultureInfo.CurrentCulture);
        }

        internal static string FormatFullNumber(long value) =>
            value.ToString("N0", CultureInfo.CurrentCulture);

        // ── HTML encoding ────────────────────────────────────────────────────────

        internal static string HtmlEncode(string? text) =>
            (text ?? string.Empty)
                .Replace("&",  "&amp;")
                .Replace("<",  "&lt;")
                .Replace(">",  "&gt;")
                .Replace("\"", "&quot;");

        // ── Placeholder substitution ─────────────────────────────────────────────

        internal static string SubstitutePlaceholders(string text, InfographicQueryResult result)
        {
            return Regex.Replace(text, "\\{(\\w+)\\}", m =>
            {
                var key = m.Groups[1].Value;
                if (string.Equals(key, "value", StringComparison.OrdinalIgnoreCase))
                    return FormatFullNumber(result.MainValue);
                if (result.Scalars != null && result.Scalars.TryGetValue(key, out var v))
                    return FormatFullNumber(v);
                if (string.Equals(key, "count", StringComparison.OrdinalIgnoreCase))
                    return FormatFullNumber(result.MainValue);
                return m.Value;
            });
        }

        // ── Tile body fragments ──────────────────────────────────────────────────

        internal static string RenderTileBody(InfographicQueryResult result)
        {
            var def = result.Definition;
            if (string.Equals(def.ChartType, "summary-tile", StringComparison.OrdinalIgnoreCase))
            {
                var text = SubstitutePlaceholders(def.Details?.Text ?? string.Empty, result);
                return $"                <div class='tile-summary'><p class='summary-text'>{HtmlEncode(text)}</p></div>";
            }
            if (string.Equals(def.ChartType, "table", StringComparison.OrdinalIgnoreCase))
                return RenderTable(result);

            return RenderBarChart(result);
        }

        internal static string RenderTileHeader(InfographicQueryResult result, bool includeMetric = true)
        {
            var def = result.Definition;
            var metricLabel = string.Equals(def.ChartType, "table", StringComparison.OrdinalIgnoreCase) ? "Total" : string.Empty;
            var metricValue = FormatMetric(PrimaryValue(result));

            var metricHtml = includeMetric
                ? $@"                    <div class='tile-metric'>
                        <div class='tile-metric-value'>{HtmlEncode(metricValue)}</div>
                        <div class='tile-metric-label'>{HtmlEncode(metricLabel)}</div>
                    </div>"
                : string.Empty;

            return $@"                <div class='tile-header'>
                    <div class='tile-title'>{HtmlEncode(def.Title)}</div>
{metricHtml}
                </div>";
        }

        internal static string RenderTable(InfographicQueryResult result)
        {
            if (result.DetailRows.Count == 0) return string.Empty;

            var def = result.Definition;
            var colNames = def.TableColumns ?? result.DetailColumnNames?.ToArray() ?? new[] { "Label", "Value" };
            var rows = result.DetailRows;

            var sb = new StringBuilder();
            sb.Append("<table class=\"tile-table\"><thead><tr>");
            foreach (var col in colNames)
                sb.Append($"<th>{HtmlEncode(col)}</th>");
            sb.Append("</tr></thead><tbody>");

            int maxRows = Math.Min(rows.Count, MaxDetailRows);
            for (int i = 0; i < maxRows; i++)
            {
                var row = rows[i];
                sb.Append("<tr>");
                for (int j = 0; j < colNames.Length && j < row.Length; j++)
                {
                    sb.Append($"<td>{HtmlEncode(row[j])}</td>");
                }
                sb.Append("</tr>");
            }
            if (rows.Count > MaxDetailRows)
            {
                sb.Append("<tr>");
                sb.Append($"<td colspan='{colNames.Length}'>(+{rows.Count - MaxDetailRows} more)</td>");
                sb.Append("</tr>");
            }
            sb.Append("</tbody></table>");
            return sb.ToString();
        }

        internal static string RenderBarChart(InfographicQueryResult result)
        {
            if (result.DetailRows.Count == 0) return string.Empty;

            var rows        = result.DetailRows;
            var displayRows = new List<(string Label, long Value)>();
            if (rows.Count <= MaxDetailRows)
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
                for (int i = 0; i < MaxDetailRows; i++)
                {
                    var row = rows[i];
                    var label = row.Length > 0 ? row[0] : "(unknown)";
                    var value = row.Length > 1 && long.TryParse(row[1], out var v) ? v : 0L;
                    displayRows.Add((label, value));
                }

                var otherLabels = new List<string>();
                long otherTotal = 0;
                for (int i = MaxDetailRows; i < rows.Count; i++)
                {
                    var row = rows[i];
                    otherLabels.Add(row.Length > 0 ? row[0] : "(unknown)");
                    if (row.Length > 1 && long.TryParse(row[1], out var v))
                        otherTotal += v;
                }
                if (otherTotal > 0)
                    displayRows.Add(($"(+{otherLabels.Count} more)", otherTotal));
            }

            var max = displayRows.Max(r => r.Value);
            if (max <= 0) return string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine("                <div class=\"tile-chart\">");
            foreach (var (label, value) in displayRows)
            {
                var pct = (int)Math.Round(value * 100.0 / max);
                sb.AppendLine("                    <div class=\"chart-item\">");
                sb.AppendLine($"                        <div class=\"chart-label\">{HtmlEncode(FormatLabel(label))}</div>");
                sb.AppendLine($"                        <div class=\"chart-bar-container\"><div class=\"chart-bar-fill\" style=\"width:{pct}%\"></div></div>");
                sb.AppendLine($"                        <div class=\"chart-value\">{FormatFullNumber(value)}</div>");
                sb.AppendLine("                    </div>");
            }
            sb.Append("                </div>");
            return sb.ToString();
        }
    }
}
