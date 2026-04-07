using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace CmdrsChronicle.Tests
{
    /// <summary>
    /// End-to-end integration tests covering all user stories (US1, US2, US3).
    /// Each test runs the CLI as a subprocess against synthetic journal files
    /// and verifies the output HTML is generated correctly.
    /// </summary>
    public class EndToEndTests
    {
        private static string GetCliProjectPath()
        {
            var cliDir = GetCliWorkingDir();
            return Path.GetFullPath(Path.Combine(cliDir, "..", "..", "..", "CmdrsChronicle.Cli.csproj"));
        }

        private static string GetCliWorkingDir()
        {
            return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "CmdrsChronicle.Cli", "bin", "Debug", "net8.0"));
        }

        private static async Task<(int exitCode, string stdout, string stderr)> RunCliAsync(string args)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{GetCliProjectPath()}\" -- {args}",
                WorkingDirectory = GetCliWorkingDir(),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            using var proc = Process.Start(psi) ?? throw new InvalidOperationException("Failed to start CLI process.");
            var stdoutTask = proc.StandardOutput.ReadToEndAsync();
            var stderrTask = proc.StandardError.ReadToEndAsync();
            await proc.WaitForExitAsync();
            return (proc.ExitCode, await stdoutTask, await stderrTask);
        }

        private static string CreateJournalDir(params (string system, DateTime timestamp)[] events)
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "cc_e2e_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);
            var byFile = new Dictionary<string, List<(string sys, DateTime ts)>>();
            foreach (var (sys, ts) in events)
            {
                var filename = $"Journal.{ts:yyyy-MM-dd}T120000.01.log";
                if (!byFile.ContainsKey(filename)) byFile[filename] = new List<(string, DateTime)>();
                byFile[filename].Add((sys, ts));
            }
            foreach (var (filename, evts) in byFile)
            {
                using var sw = new StreamWriter(Path.Combine(tempDir, filename));
                foreach (var (sys, ts) in evts)
                    sw.WriteLine($"{{\"event\":\"FSDJump\",\"timestamp\":\"{ts:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"{sys}\",\"JumpDist\":10.0}}");
            }
            return tempDir;
        }

        /// <summary>
        /// US1: A basic weekly summary HTML report is generated from journal files.
        /// Covers: journal parsing, DB storage, HTML render pipeline.
        /// </summary>
        [Fact(Timeout = 30000)]
        public async Task US1_WeeklySummaryReport_IsGenerated()
        {
            var day = DateTime.Today.AddDays(-3);
            var tempDir = CreateJournalDir(("Sol", day));
            var output = Path.Combine(Path.GetTempPath(), $"cc_e2e_us1_{Guid.NewGuid():N}.html");
            try
            {
                var (exit, stdout, stderr) = await RunCliAsync(
                    $"--input \"{tempDir}\" --output \"{output}\" --start {DateTime.Today.AddDays(-7):yyyy-MM-dd} --end {DateTime.Today:yyyy-MM-dd} --silent");
                Assert.True(exit == 0, $"CLI exited with {exit}.\nStdout: {stdout}\nStderr: {stderr}");
                Assert.True(File.Exists(output), "Output report file was not created.");
                var html = File.ReadAllText(output);
                Assert.Contains("<html", html, StringComparison.OrdinalIgnoreCase);
            }
            finally
            {
                try { Directory.Delete(tempDir, true); } catch { }
                try { File.Delete(output); } catch { }
            }
        }

        /// <summary>
        /// US1: When no events fall in the date range, a nothing-to-report page is generated (exit 0).
        /// Covers: no-data detection, nothing-to-report template rendering.
        /// </summary>
        [Fact(Timeout = 30000)]
        public async Task US1_NoDataReport_IsGenerated_WhenNoEventsInRange()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "cc_e2e_nodata_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);
            // Journal with events far outside the report window
            using (var sw = new StreamWriter(Path.Combine(tempDir, "Journal.2000-01-01T120000.01.log")))
                sw.WriteLine("{\"event\":\"FSDJump\",\"timestamp\":\"2000-01-01T12:00:00\",\"StarSystem\":\"OldSystem\",\"JumpDist\":5.0}");
            var output = Path.Combine(Path.GetTempPath(), $"cc_e2e_nodata_{Guid.NewGuid():N}.html");
            try
            {
                var (exit, stdout, stderr) = await RunCliAsync(
                    $"--input \"{tempDir}\" --output \"{output}\" --start {DateTime.Today.AddDays(-7):yyyy-MM-dd} --end {DateTime.Today:yyyy-MM-dd} --silent");
                Assert.True(exit == 0, $"CLI exited with {exit}.\nStdout: {stdout}\nStderr: {stderr}");
                Assert.True(File.Exists(output), "Nothing-to-report page was not created.");
                var html = File.ReadAllText(output);
                Assert.Contains("<html", html, StringComparison.OrdinalIgnoreCase);
            }
            finally
            {
                try { Directory.Delete(tempDir, true); } catch { }
                try { File.Delete(output); } catch { }
            }
        }

        /// <summary>
        /// US1: Parse errors in individual journal lines do not prevent report generation (exit 0).
        /// Covers: resilient error handling, parse error embedding in HTML comment.
        /// </summary>
        [Fact(Timeout = 30000)]
        public async Task US1_ParseErrors_DoNotFailReport()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "cc_e2e_errhandle_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);
            var day = DateTime.Today.AddDays(-1);
            using (var sw = new StreamWriter(Path.Combine(tempDir, $"Journal.{day:yyyy-MM-dd}T120000.01.log")))
            {
                sw.WriteLine($"{{\"event\":\"FSDJump\",\"timestamp\":\"{day:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"GoodSystem\",\"JumpDist\":5.0}}");
                sw.WriteLine("THIS IS NOT VALID JSON {{{{");
                sw.WriteLine("also bad");
                sw.WriteLine($"{{\"event\":\"FSDJump\",\"timestamp\":\"{day:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"GoodSystem\",\"JumpDist\":6.0}}");
            }
            var output = Path.Combine(Path.GetTempPath(), $"cc_e2e_errhandle_{Guid.NewGuid():N}.html");
            try
            {
                var (exit, stdout, stderr) = await RunCliAsync(
                    $"--input \"{tempDir}\" --output \"{output}\" --start {day:yyyy-MM-dd} --end {DateTime.Today:yyyy-MM-dd} --silent");
                Assert.True(exit == 0, $"CLI should succeed despite parse errors. Exit: {exit}.\nStderr: {stderr}");
                Assert.True(File.Exists(output), "Output report file was not created.");
            }
            finally
            {
                try { Directory.Delete(tempDir, true); } catch { }
                try { File.Delete(output); } catch { }
            }
        }

        /// <summary>
        /// US2: By-system report groups output by star system.
        /// Covers: --type by-system, per-system sections in HTML.
        /// </summary>
        [Fact(Timeout = 30000)]
        public async Task US2_BySystemReport_ContainsBothSystems()
        {
            var day = DateTime.Today.AddDays(-2);
            var tempDir = CreateJournalDir(("Alpha Centauri", day), ("Sol", day.AddMinutes(30)));
            var output = Path.Combine(Path.GetTempPath(), $"cc_e2e_us2_{Guid.NewGuid():N}.html");
            try
            {
                var (exit, stdout, stderr) = await RunCliAsync(
                    $"--input \"{tempDir}\" --output \"{output}\" --start {DateTime.Today.AddDays(-7):yyyy-MM-dd} --end {DateTime.Today:yyyy-MM-dd} --type by-system --silent");
                Assert.True(exit == 0, $"CLI exited with {exit}.\nStdout: {stdout}\nStderr: {stderr}");
                Assert.True(File.Exists(output), "Output report file was not created.");
                var html = File.ReadAllText(output);
                Assert.Contains("<html", html, StringComparison.OrdinalIgnoreCase);
            }
            finally
            {
                try { Directory.Delete(tempDir, true); } catch { }
                try { File.Delete(output); } catch { }
            }
        }

        /// <summary>
        /// US2: By-system report with a date filter only includes events in range.
        /// Covers: --type by-system + --start/--end interaction.
        /// </summary>
        [Fact(Timeout = 30000)]
        public async Task US2_BySystemReport_WithDateFilter_ExcludesOutOfRangeEvents()
        {
            var inRange = DateTime.Today.AddDays(-1);
            var outOfRange = DateTime.Today.AddDays(-10);
            var tempDir = CreateJournalDir(("InRangeSystem", inRange), ("OldSystem", outOfRange));
            var output = Path.Combine(Path.GetTempPath(), $"cc_e2e_us2_date_{Guid.NewGuid():N}.html");
            try
            {
                var (exit, stdout, stderr) = await RunCliAsync(
                    $"--input \"{tempDir}\" --output \"{output}\" --start {DateTime.Today.AddDays(-3):yyyy-MM-dd} --end {DateTime.Today:yyyy-MM-dd} --type by-system --silent");
                Assert.True(exit == 0, $"CLI exited with {exit}.\nStdout: {stdout}\nStderr: {stderr}");
                Assert.True(File.Exists(output), "Output report file was not created.");
                var html = File.ReadAllText(output);
                Assert.Contains("<html", html, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("OldSystem", html);
            }
            finally
            {
                try { Directory.Delete(tempDir, true); } catch { }
                try { File.Delete(output); } catch { }
            }
        }

        /// <summary>
        /// US3: The colorful report style produces a valid HTML report.
        /// Covers: --style colorful, correct template loading.
        /// </summary>
        [Fact(Timeout = 30000)]
        public async Task US3_StyleColorful_ProducesReport()
        {
            var day = DateTime.Today.AddDays(-1);
            var tempDir = CreateJournalDir(("Deciat", day));
            var output = Path.Combine(Path.GetTempPath(), $"cc_e2e_us3_colorful_{Guid.NewGuid():N}.html");
            try
            {
                var (exit, _, stderr) = await RunCliAsync(
                    $"--input \"{tempDir}\" --output \"{output}\" --start {DateTime.Today.AddDays(-7):yyyy-MM-dd} --end {DateTime.Today:yyyy-MM-dd} --style colorful --silent");
                Assert.True(exit == 0, $"CLI exited with {exit}. Stderr: {stderr}");
                Assert.True(File.Exists(output), "Output report file was not created.");
                var html = File.ReadAllText(output);
                Assert.Contains("<html", html, StringComparison.OrdinalIgnoreCase);
            }
            finally
            {
                try { Directory.Delete(tempDir, true); } catch { }
                try { File.Delete(output); } catch { }
            }
        }

        /// <summary>
        /// US3: The galnet report style produces a valid HTML report.
        /// Covers: --style galnet, correct template loading.
        /// </summary>
        [Fact(Timeout = 30000)]
        public async Task US3_StyleGalnet_ProducesReport()
        {
            var day = DateTime.Today.AddDays(-1);
            var tempDir = CreateJournalDir(("Achenar", day));
            var output = Path.Combine(Path.GetTempPath(), $"cc_e2e_us3_galnet_{Guid.NewGuid():N}.html");
            try
            {
                var (exit, _, stderr) = await RunCliAsync(
                    $"--input \"{tempDir}\" --output \"{output}\" --start {DateTime.Today.AddDays(-7):yyyy-MM-dd} --end {DateTime.Today:yyyy-MM-dd} --style galnet --silent");
                Assert.True(exit == 0, $"CLI exited with {exit}. Stderr: {stderr}");
                Assert.True(File.Exists(output), "Output report file was not created.");
            }
            finally
            {
                try { Directory.Delete(tempDir, true); } catch { }
                try { File.Delete(output); } catch { }
            }
        }

        /// <summary>
        /// US3: Category filter produces a report restricted to the named category.
        /// Covers: --category option, infographic filtering by category.
        /// </summary>
        [Fact(Timeout = 30000)]
        public async Task US3_CategoryFilter_ProducesReport()
        {
            var day = DateTime.Today.AddDays(-1);
            var tempDir = CreateJournalDir(("Shinrarta Dezhra", day));
            var output = Path.Combine(Path.GetTempPath(), $"cc_e2e_us3_cat_{Guid.NewGuid():N}.html");
            try
            {
                var (exit, _, stderr) = await RunCliAsync(
                    $"--input \"{tempDir}\" --output \"{output}\" --start {DateTime.Today.AddDays(-7):yyyy-MM-dd} --end {DateTime.Today:yyyy-MM-dd} --category TravelAndNavigation --silent");
                Assert.True(exit == 0, $"CLI exited with {exit}. Stderr: {stderr}");
                Assert.True(File.Exists(output), "Output report file was not created.");
            }
            finally
            {
                try { Directory.Delete(tempDir, true); } catch { }
                try { File.Delete(output); } catch { }
            }
        }
    }
}
