using System;
using System.Collections.Generic;
using System.IO;
using CmdrsChronicle.Core;
using Xunit;

namespace CmdrsChronicle.Core.Tests
{
    public class ElegantReportRendererTests
    {
        private static (string templatePath, string cssPath, DirectoryInfo tmpDir) CreateTempTemplates()
        {
            var tmp = Directory.CreateTempSubdirectory("renderer_test_");
            var tmpl = Path.Combine(tmp.FullName, "elegant-report.html");
            var css  = Path.Combine(tmp.FullName, "elegant.css");

            File.WriteAllText(tmpl, "<html><head><style>{CSS}</style></head><body>" +
                "<h1>{CMDR_NAME}</h1><p>{TAGLINE}</p>" +
                "<span>{DATE_FROM}</span><span>{DATE_TO}</span>" +
                "<div>{TILES}</div>" +
                "<footer>{DATE_GENERATED}</footer>" +
                "</body></html>");
            File.WriteAllText(css, "body{color:gold}");

            return (tmpl, css, tmp);
        }

        [Fact]
        public void Render_ContainsCmdrNameAndDates()
        {
            var (tmpl, css, tmp) = CreateTempTemplates();
            try
            {
                var html = ElegantReportRenderer.Render(tmpl, css, "tagline", "Stellar Fox",
                    "3312-01-01", "3312-01-07", new[] { new SystemVisit(null, null, null, Array.Empty<InfographicQueryResult>()) });

                Assert.Contains("Stellar Fox",  html);
                Assert.Contains("3312-01-01",   html);
                Assert.Contains("3312-01-07",   html);
            }
            finally { tmp.Delete(true); }
        }

        [Fact]
        public void Render_InlinesCSS()
        {
            var (tmpl, css, tmp) = CreateTempTemplates();
            try
            {
                var html = ElegantReportRenderer.Render(tmpl, css, "t", "CMDR",
                    "3312-01-01", "3312-01-07", new[] { new SystemVisit(null, null, null, Array.Empty<InfographicQueryResult>()) });

                Assert.Contains("body{color:gold}", html);
            }
            finally { tmp.Delete(true); }
        }

        [Fact]
        public void Render_IncludesTile_WhenMeetsThreshold()
        {
            var (tmpl, css, tmp) = CreateTempTemplates();
            try
            {
                var def = new InfographicDefinition { Category = "Mining", Title = "Minerals Refined", Query = "", Threshold = 1, Enabled = true };
                var results = new List<InfographicQueryResult>
                {
                    new() { Definition = def, MainValue = 5, DetailRows = Array.Empty<string[]>() }
                };

                var html = ElegantReportRenderer.Render(tmpl, css, "t", "CMDR",
                    "3312-01-01", "3312-01-07", new[] { new SystemVisit(null, null, null, results) });

                Assert.Contains("Minerals Refined", html);
                Assert.Contains("Mining",           html);
            }
            finally { tmp.Delete(true); }
        }

        [Fact]
        public void Render_ExcludesTile_WhenBelowThreshold()
        {
            var (tmpl, css, tmp) = CreateTempTemplates();
            try
            {
                var def = new InfographicDefinition { Category = "Mining", Title = "Secret Tile", Query = "", Threshold = 10, Enabled = true };
                var results = new List<InfographicQueryResult>
                {
                    new() { Definition = def, MainValue = 0, DetailRows = Array.Empty<string[]>() }
                };

                var html = ElegantReportRenderer.Render(tmpl, css, "t", "CMDR",
                    "3312-01-01", "3312-01-07", new[] { new SystemVisit(null, null, null, results) });

                Assert.DoesNotContain("Secret Tile", html);
            }
            finally { tmp.Delete(true); }
        }

        [Fact]
        public void Render_ShowsFallbackMessage_WhenNoTilesQualify()
        {
            var (tmpl, css, tmp) = CreateTempTemplates();
            try
            {
                var html = ElegantReportRenderer.Render(tmpl, css, "t", "CMDR",
                    "3312-01-01", "3312-01-07", new[] { new SystemVisit(null, null, null, Array.Empty<InfographicQueryResult>()) });

                Assert.Contains("No qualifying activity", html);
            }
            finally { tmp.Delete(true); }
        }

        [Fact]
        public void Render_RendersBarChart_WhenDetailRowsExist()
        {
            var (tmpl, css, tmp) = CreateTempTemplates();
            try
            {
                var def = new InfographicDefinition { Category = "Mining", Title = "Minerals", Query = "", Threshold = 1, ChartType = "bar-chart", Enabled = true };
                var rows = new List<string[]> { new[] { "Gold", "10" }, new[] { "Platinum", "5" } };
                var results = new List<InfographicQueryResult>
                {
                    new() { Definition = def, MainValue = 15, DetailRows = rows }
                };

                var html = ElegantReportRenderer.Render(tmpl, css, "t", "CMDR",
                    "3312-01-01", "3312-01-07", new[] { new SystemVisit(null, null, null, results) });

                Assert.Contains("Gold",           html);
                Assert.Contains("Platinum",       html);
                Assert.Contains("chart-bar-fill", html);
                Assert.Contains("width:100%",     html); // Gold is max → 100%
                Assert.Contains("width:50%",      html); // Platinum is 50% of Gold
            }
            finally { tmp.Delete(true); }
        }

        [Fact]
        public void Render_RollsUpExcessRowsIntoOther()
        {
            var (tmpl, css, tmp) = CreateTempTemplates();
            try
            {
                var def = new InfographicDefinition { Category = "Mining", Title = "Minerals", Query = "", Threshold = 1, ChartType = "bar-chart", Enabled = true };
                var rows = new List<string[]>
                {
                    new[] { "Gold",        "100" },
                    new[] { "Platinum",     "80" },
                    new[] { "Painite",      "60" },
                    new[] { "Osmium",       "40" },
                    new[] { "Lithium",      "20" },
                    new[] { "Bertrandite",  "10" },
                    new[] { "Bromellite",    "5" },
                };
                var results = new List<InfographicQueryResult>
                {
                    new() { Definition = def, MainValue = 315, DetailRows = rows }
                };

                var html = ElegantReportRenderer.Render(tmpl, css, "t", "CMDR",
                    "3312-01-01", "3312-01-07", new[] { new SystemVisit(null, null, null, results) });

                Assert.Contains("Gold",          html);
                Assert.Contains("Platinum",      html);
                Assert.Contains("Painite",       html);
                Assert.Contains("Osmium",        html);
                Assert.Contains("Lithium",       html);
                Assert.Contains("Other",         html);  // rolled-up row present
                Assert.Contains("15",            html);  // 10 + 5 = 15
                Assert.DoesNotContain("Bertrandite", html);
                Assert.DoesNotContain("Bromellite",  html);
            }
            finally { tmp.Delete(true); }
        }

        [Fact]
        public void Render_HtmlEncodesSpecialCharacters()
        {
            var (tmpl, css, tmp) = CreateTempTemplates();
            try
            {
                var def = new InfographicDefinition { Category = "Test & Demo", Title = "<Alert>", Query = "", Threshold = 1, Enabled = true };
                var results = new List<InfographicQueryResult>
                {
                    new() { Definition = def, MainValue = 1, DetailRows = Array.Empty<string[]>() }
                };

                var html = ElegantReportRenderer.Render(tmpl, css, "t", "CMDR",
                    "3312-01-01", "3312-01-07", new[] { new SystemVisit(null, null, null, results) });

                Assert.Contains("&lt;Alert&gt;",   html);
                Assert.Contains("Test &amp; Demo", html);
                Assert.DoesNotContain("<Alert>",   html);
            }
            finally { tmp.Delete(true); }
        }
    }
}
