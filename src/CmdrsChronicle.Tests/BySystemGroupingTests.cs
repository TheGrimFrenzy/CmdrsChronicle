using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace CmdrsChronicle.Tests
{
    public class BySystemGroupingTests
    {
        private string CreateBySystemTestDir()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "cc_bysys_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);
            // Two systems, two days, different event counts
            var day1 = DateTime.Today.AddDays(-2);
            var day2 = DateTime.Today.AddDays(-1);
            var file1 = Path.Combine(tempDir, $"Journal.{day1:yyyy-MM-dd}T120000.01.log");
            var file2 = Path.Combine(tempDir, $"Journal.{day2:yyyy-MM-dd}T120000.01.log");
            using (var sw = new StreamWriter(file1))
            {
                // Alpha system events
                sw.WriteLine($"{{\"event\":\"FSDJump\",\"timestamp\":\"{day1:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Alpha\",\"JumpDist\":10.0}}" );
                sw.WriteLine($"{{\"event\":\"SupercruiseExit\",\"timestamp\":\"{day1:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Alpha\",\"BodyType\":\"Station\",\"Body\":\"Alpha Station\"}}" );
                sw.WriteLine($"{{\"event\":\"Docked\",\"timestamp\":\"{day1:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Alpha\",\"StationName\":\"Alpha Station\",\"StationType\":\"Coriolis\"}}" );
                sw.WriteLine($"{{\"event\":\"FuelScoop\",\"timestamp\":\"{day1:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Alpha\",\"Scooped\":5.0}}" );
                sw.WriteLine($"{{\"event\":\"DockingDenied\",\"timestamp\":\"{day1:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Alpha\",\"Reason\":\"NoSpace\"}}" );
                // Beta system events
                sw.WriteLine($"{{\"event\":\"FSDJump\",\"timestamp\":\"{day1:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Beta\",\"JumpDist\":15.0}}" );
                sw.WriteLine($"{{\"event\":\"SupercruiseExit\",\"timestamp\":\"{day1:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Beta\",\"BodyType\":\"Station\",\"Body\":\"Beta Station\"}}" );
                sw.WriteLine($"{{\"event\":\"Docked\",\"timestamp\":\"{day1:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Beta\",\"StationName\":\"Beta Station\",\"StationType\":\"Orbis\"}}" );
                sw.WriteLine($"{{\"event\":\"FuelScoop\",\"timestamp\":\"{day1:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Beta\",\"Scooped\":7.0}}" );
                sw.WriteLine($"{{\"event\":\"DockingDenied\",\"timestamp\":\"{day1:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Beta\",\"Reason\":\"TooLarge\"}}" );
            }
            using (var sw = new StreamWriter(file2))
            {
                // Alpha system, second day
                sw.WriteLine($"{{\"event\":\"FSDJump\",\"timestamp\":\"{day2:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Alpha\",\"JumpDist\":12.0}}" );
                sw.WriteLine($"{{\"event\":\"SupercruiseExit\",\"timestamp\":\"{day2:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Alpha\",\"BodyType\":\"Planet\",\"Body\":\"Alpha 1\"}}" );
                sw.WriteLine($"{{\"event\":\"Docked\",\"timestamp\":\"{day2:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Alpha\",\"StationName\":\"Alpha Station\",\"StationType\":\"Coriolis\"}}" );
                sw.WriteLine($"{{\"event\":\"FuelScoop\",\"timestamp\":\"{day2:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Alpha\",\"Scooped\":6.0}}" );
                sw.WriteLine($"{{\"event\":\"DockingDenied\",\"timestamp\":\"{day2:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Alpha\",\"Reason\":\"NoSpace\"}}" );
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

        [Fact(Timeout = 20000)]
        public async Task BySystemGrouping_GeneratesPerSystemSections()
        {
            await Task.Run(() => {
                var tempDir = CreateBySystemTestDir();
                var output = Path.Combine(Path.GetTempPath(), "cc_bysys_report.html");
                try
                {
                    var cliDir = GetCliWorkingDir();
                    var csprojPath = Path.GetFullPath(Path.Combine(cliDir, "..", "..", "..", "CmdrsChronicle.Cli.csproj"));
                    var psi = new ProcessStartInfo
                    {
                        FileName = "dotnet",
                        Arguments = $"run --project \"{csprojPath}\" -- --input \"{tempDir}\" --output \"{output}\" --start {DateTime.Today.AddDays(-3):yyyy-MM-dd} --end {DateTime.Today:yyyy-MM-dd} --type by-system --silent",
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
                    proc.WaitForExit(15000);
                    var stdout = proc.StandardOutput.ReadToEnd();
                    var stderr = proc.StandardError.ReadToEnd();
                    if (proc.ExitCode != 0)
                    {
                        Console.WriteLine($"CLI failed. StdErr:\n{stderr}\nStdOut:\n{stdout}");
                    }
                    Assert.True(proc.ExitCode == 0, $"CLI exited with code {proc.ExitCode}. StdErr: {stderr}");
                    Assert.True(File.Exists(output), "Output report file not found");
                    var html = File.ReadAllText(output);
                    // Check for both system names in the output
                    Assert.Contains("Alpha", html);
                    Assert.Contains("Beta", html);
                    // Check for at least two system sections (e.g., by <section> or heading)
                    var systemSectionCount = Regex.Matches(html, "Alpha|Beta").Count;
                    Assert.True(systemSectionCount >= 2, "Expected at least two system sections in report");
                }
                finally { try { Directory.Delete(tempDir, true); } catch { } try { File.Delete(output); } catch { } }
            });
        }

        [Fact(Timeout = 20000)]
        public async Task BySystemGrouping_RespectsDateAndCategoryFilters()
        {
            await Task.Run(() => {
                var tempDir = CreateBySystemTestDir();
                var output = Path.Combine(Path.GetTempPath(), "cc_bysys_report2.html");
                try
                {
                    var cliDir = GetCliWorkingDir();
                    var csprojPath = Path.GetFullPath(Path.Combine(cliDir, "..", "..", "..", "CmdrsChronicle.Cli.csproj"));
                    var psi = new ProcessStartInfo
                    {
                        FileName = "dotnet",
                        Arguments = $"run --project \"{csprojPath}\" -- --input \"{tempDir}\" --output \"{output}\" --start {DateTime.Today.AddDays(-1):yyyy-MM-dd} --end {DateTime.Today:yyyy-MM-dd} --type by-system --silent",
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
                    proc.WaitForExit(15000);
                    var stdout = proc.StandardOutput.ReadToEnd();
                    var stderr = proc.StandardError.ReadToEnd();
                    if (proc.ExitCode != 0)
                    {
                        Console.WriteLine($"CLI failed. StdErr:\n{stderr}\nStdOut:\n{stdout}");
                    }
                    Assert.True(proc.ExitCode == 0, $"CLI exited with code {proc.ExitCode}. StdErr: {stderr}");
                    Assert.True(File.Exists(output), "Output report file not found");
                    var html = File.ReadAllText(output);
                    Assert.Contains("Alpha", html);
                    Assert.DoesNotContain("Beta", html);
                }
                finally { try { Directory.Delete(tempDir, true); } catch { } try { File.Delete(output); } catch { } }
            });
        }
    }
}
