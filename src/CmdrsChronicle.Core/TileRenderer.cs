using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CmdrsChronicle.Core
{
    /// <summary>
    /// Shared tile-rendering helpers used by both <see cref="ElegantReportRenderer"/>
    /// and <see cref="ColorfulReportRenderer"/>.
    /// </summary>
    internal static class TileRenderer
    {
        internal const int MaxDetailRows = 5;

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

            var slug    = NormalizeCategory(category);
            var allowed = new[] { "travel", "combat", "exploration", "exobiology", "trade",
                                  "powerplay", "fleetcarriers", "colonisation", "onfoot",
                                  "mining", "engineering", "crew" };

            string? chosen = null;
            foreach (var a in allowed) if (slug == a)          { chosen = a; break; }
            if (chosen == null)
                foreach (var a in allowed) if (slug.Contains(a)) { chosen = a; break; }
            if (chosen == null)
            {
                var parts = slug.Split('-');
                foreach (var p in parts) if (allowed.Contains(p)) { chosen = p; break; }
                if (chosen == null) chosen = slug.Split('-').FirstOrDefault() ?? slug;
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

            var def       = result.Definition;
            var colLabel  = def.TableColumns?.Length >= 1 ? def.TableColumns[0] : "Label";
            var colValue  = def.TableColumns?.Length >= 2 ? def.TableColumns[1] : "Value";
            var inaraBase = def.InaraSearchBase;

            var rows        = result.DetailRows;
            var displayRows = new List<(string Label, long Value)>();
            var otherLabels = new List<string>();
            long otherTotal = 0;

            if (rows.Count <= MaxDetailRows)
            {
                displayRows.AddRange(rows);
            }
            else
            {
                for (int i = 0; i < MaxDetailRows; i++)
                    displayRows.Add(rows[i]);
                for (int i = MaxDetailRows; i < rows.Count; i++)
                {
                    otherLabels.Add(rows[i].Label);
                    otherTotal += rows[i].Value;
                }
                displayRows.Add(("Other", otherTotal));
            }

            var sb = new StringBuilder();
            sb.Append($"<table class=\"tile-table\"><thead><tr><th>{HtmlEncode(colLabel)}</th><th>{HtmlEncode(colValue)}</th></tr></thead><tbody>");

            foreach (var (label, value) in displayRows)
            {
                string labelCell;
                if (label == "Other" && otherLabels.Count > 0)
                {
                    var catchAllLabel = $"(+{otherLabels.Count} more)";
                    labelCell = $"<td title=\"{HtmlEncode(string.Join(", ", otherLabels.Select(FormatLabel)))}\">{HtmlEncode(catchAllLabel)}</td>";
                }
                else if (inaraBase != null)
                {
                    var href = HtmlEncode(inaraBase + Uri.EscapeDataString(label));
                    labelCell = $"<td><a href=\"{href}\" target=\"_blank\" class=\"inara-link\"><span class=\"inara-icon\"></span></a>{HtmlEncode(FormatLabel(label))}</td>";
                }
                else
                {
                    labelCell = $"<td>{HtmlEncode(FormatLabel(label))}</td>";
                }
                sb.Append($"<tr>{labelCell}<td>{FormatFullNumber(value)}</td></tr>");
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
                displayRows.AddRange(rows);
            }
            else
            {
                for (int i = 0; i < MaxDetailRows; i++)
                    displayRows.Add(rows[i]);

                var otherLabels = new List<string>();
                long otherTotal = 0;
                for (int i = MaxDetailRows; i < rows.Count; i++)
                {
                    otherLabels.Add(rows[i].Label);
                    otherTotal += rows[i].Value;
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
