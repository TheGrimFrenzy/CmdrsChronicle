using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CmdrsChronicle.Tests
{
    public class PerformanceAndMemoryTests
    {
        private string CreateTempJournalDir(int days, int eventsPerDay)
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "cc_perf_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);
            var start = DateTime.Today.AddDays(-days);
            for (int d = 0; d < days; d++)
            {
                var date = start.AddDays(d);
                var file = Path.Combine(tempDir, $"Journal.{date:yyyy-MM-dd}T120000.01.log");
                using var sw = new StreamWriter(file);
                for (int e = 0; e < eventsPerDay; e++)
                {
                    var ts = date.AddMinutes(e);
                    sw.WriteLine($"{{\"event\":\"FSDJump\",\"timestamp\":\"{ts:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"PerfSys{d}\"}}");
                }
            }
            return tempDir;
        }

        private static string GetCliWorkingDir()
        {
            var baseDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "CmdrsChronicle.Cli", "bin"));
            var releaseDir = Path.Combine(baseDir, "Release", "net8.0");
            var debugDir = Path.Combine(baseDir, "Debug", "net8.0");
            if (Directory.Exists(releaseDir)) return releaseDir;
            if (Directory.Exists(debugDir)) return debugDir;
            throw new DirectoryNotFoundException($"Neither Release nor Debug CLI output found. Checked: {releaseDir} and {debugDir}");
        }

        [Fact(Timeout = 35000)]
        public async Task ReportGeneration_CompletesUnder30Seconds_ForOneWeek()
        {
            await Task.Run(async () => {
                var tempDir = CreateTempJournalDir(7, 100); // 7 days, 100 events/day
                var output = Path.Combine(Path.GetTempPath(), "cc_perf_report.html");
                try
                {
                    var cliDir = GetCliWorkingDir();
                    var csprojPath = Path.GetFullPath(Path.Combine(cliDir, "..", "..", "..", "CmdrsChronicle.Cli.csproj"));
                    var psi = new ProcessStartInfo
                    {
                        FileName = "dotnet",
                        Arguments = $"run --project \"{csprojPath}\" -- --input \"{tempDir}\" --output \"{output}\" --start {DateTime.Today.AddDays(-7):yyyy-MM-dd} --end {DateTime.Today:yyyy-MM-dd} --silent",
                        WorkingDirectory = cliDir,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false
                    };
                    using var proc = Process.Start(psi);
                    if (proc == null)
                    {
                        throw new InvalidOperationException("Failed to start process for CLI test.");
                    }
                    var sw = Stopwatch.StartNew();
                    var stdOutTask = proc.StandardOutput.ReadToEndAsync();
                    var stdErrTask = proc.StandardError.ReadToEndAsync();
                    await proc.WaitForExitAsync();
                    sw.Stop();
                    var stdout = await stdOutTask;
                    var stderr = await stdErrTask;
                    Console.WriteLine("[CLI STDOUT]\n" + stdout);
                    Console.WriteLine("[CLI STDERR]\n" + stderr);
                    if (proc.ExitCode != 0)
                    {
                        Console.WriteLine($"CLI failed. Exit code: {proc.ExitCode}");
                    }
                    Assert.True(proc.ExitCode == 0, $"CLI exited with code {proc.ExitCode}. StdErr: {stderr}");
                    Assert.True(sw.Elapsed.TotalSeconds < 30, $"Report generation took {sw.Elapsed.TotalSeconds:F1}s (should be <30s)");
                    Assert.True(File.Exists(output), "Output report file not found");
                }
                finally { try { Directory.Delete(tempDir, true); } catch { } try { File.Delete(output); } catch { } }
            });
        }

        [Fact(Timeout = 35000)]
        public async Task ReportGeneration_MemoryUsage_Under500MB_For10000Events()
        {
            await Task.Run(() => {
                var tempDir = CreateTempJournalDir(1, 10000); // 1 day, 10,000 events
                var output = Path.Combine(Path.GetTempPath(), "cc_mem_report.html");
                try
                {
                    var cliDir = GetCliWorkingDir();
                    var csprojPath = Path.GetFullPath(Path.Combine(cliDir, "..", "..", "..", "CmdrsChronicle.Cli.csproj"));
                    var psi = new ProcessStartInfo
                    {
                        FileName = "dotnet",
                        Arguments = $"run --project \"{csprojPath}\" -- --input \"{tempDir}\" --output \"{output}\" --start {DateTime.Today.AddDays(-7):yyyy-MM-dd} --end {DateTime.Today:yyyy-MM-dd} --silent",
                        WorkingDirectory = cliDir,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false
                    };
                    long peakMemory = 0;
                    using var proc = Process.Start(psi);
                    if (proc == null)
                    {
                        throw new InvalidOperationException("Failed to start process for CLI test.");
                    }
                    while (!proc.HasExited)
                    {
                        try { peakMemory = Math.Max(peakMemory, proc.PeakWorkingSet64); } catch { }
                        Task.Delay(100).Wait();
                    }
                    var stdout = proc.StandardOutput.ReadToEnd();
                    var stderr = proc.StandardError.ReadToEnd();
                    if (proc.ExitCode != 0)
                    {
                        Console.WriteLine($"CLI failed. StdErr:\n{stderr}\nStdOut:\n{stdout}");
                    }
                    Assert.True(proc.ExitCode == 0, $"CLI exited with code {proc.ExitCode}. StdErr: {stderr}");
                    Assert.True(peakMemory < 500 * 1024 * 1024, $"Peak memory usage {peakMemory / (1024 * 1024)}MB exceeded 500MB");
                    Assert.True(File.Exists(output), "Output report file not found");
                }
                finally { try { Directory.Delete(tempDir, true); } catch { } try { File.Delete(output); } catch { } }
            });
        }
    }
}
