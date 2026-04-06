using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace CmdrsChronicle.Tests
{
    public class BySystemCombinedFilterTests
    {
        private string CreateBySystemCategoryTestDir()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "cc_bysyscat_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);
            // Two systems, two categories, two days
            var day1 = DateTime.Today.AddDays(-2);
            var day2 = DateTime.Today.AddDays(-1);
            var file1 = Path.Combine(tempDir, $"Journal.{day1:yyyy-MM-dd}T120000.01.log");
            var file2 = Path.Combine(tempDir, $"Journal.{day2:yyyy-MM-dd}T120000.01.log");
            // Assume 'Travel' and 'Combat' are valid categories in infographics
            using (var sw = new StreamWriter(file1))
            {
                // Alpha system events (Travel and Combat)
                sw.WriteLine($"{{\"event\":\"FSDJump\",\"timestamp\":\"{day1:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Alpha\",\"JumpDist\":10.0,\"SystemAddress\":1,\"StarPos\":\"[0,0,0]\",\"SystemAllegiance\":\"Federation\",\"SystemEconomy\":\"Industrial\",\"SystemEconomy_Localised\":\"Industrial\",\"SystemSecondEconomy\":\"None\",\"SystemSecondEconomy_Localised\":\"None\",\"SystemGovernment\":\"Democracy\",\"SystemGovernment_Localised\":\"Democracy\",\"SystemSecurity\":\"High\",\"SystemSecurity_Localised\":\"High\",\"Population\":1000,\"Body\":\"Alpha 1\",\"BodyID\":1,\"BodyType\":\"Star\",\"FuelUsed\":1.0,\"FuelLevel\":10.0,\"event_timestamp\":\"{day1:yyyy-MM-ddTHH:mm:ss}\"}}" );
                sw.WriteLine($"{{\"event\":\"SupercruiseExit\",\"timestamp\":\"{day1:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Alpha\",\"BodyType\":\"Station\",\"Body\":\"Alpha Station\"}}" );
                sw.WriteLine($"{{\"event\":\"Docked\",\"timestamp\":\"{day1:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Alpha\",\"StationName\":\"Alpha Station\",\"StationType\":\"Coriolis\"}}" );
                sw.WriteLine($"{{\"event\":\"FuelScoop\",\"timestamp\":\"{day1:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Alpha\",\"Scooped\":5.0}}" );
                sw.WriteLine($"{{\"event\":\"DockingDenied\",\"timestamp\":\"{day1:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Alpha\",\"Reason\":\"NoSpace\"}}" );
                sw.WriteLine($"{{\"event\":\"Bounty\",\"timestamp\":\"{day1:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Alpha\"}}" );
                // Beta system events (Travel and Combat)
                // FSDJump for Beta is now at 11:59:00 to ensure it's before the window
                sw.WriteLine($"{{\"event\":\"FSDJump\",\"timestamp\":\"{day1:yyyy-MM-dd}T11:59:00\",\"StarSystem\":\"Beta\",\"JumpDist\":15.0,\"SystemAddress\":2,\"StarPos\":\"[1,1,1]\",\"SystemAllegiance\":\"Empire\",\"SystemEconomy\":\"Agriculture\",\"SystemEconomy_Localised\":\"Agriculture\",\"SystemSecondEconomy\":\"None\",\"SystemSecondEconomy_Localised\":\"None\",\"SystemGovernment\":\"Dictatorship\",\"SystemGovernment_Localised\":\"Dictatorship\",\"SystemSecurity\":\"Low\",\"SystemSecurity_Localised\":\"Low\",\"Population\":2000,\"Body\":\"Beta 1\",\"BodyID\":2,\"BodyType\":\"Star\",\"FuelUsed\":2.0,\"FuelLevel\":20.0,\"event_timestamp\":\"{day1:yyyy-MM-dd}T11:59:00\"}}" );
                sw.WriteLine($"{{\"event\":\"SupercruiseExit\",\"timestamp\":\"{day1:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Beta\",\"BodyType\":\"Station\",\"Body\":\"Beta Station\"}}" );
                sw.WriteLine($"{{\"event\":\"Docked\",\"timestamp\":\"{day1:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Beta\",\"StationName\":\"Beta Station\",\"StationType\":\"Orbis\"}}" );
                sw.WriteLine($"{{\"event\":\"FuelScoop\",\"timestamp\":\"{day1:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Beta\",\"Scooped\":7.0}}" );
                sw.WriteLine($"{{\"event\":\"DockingDenied\",\"timestamp\":\"{day1:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Beta\",\"Reason\":\"TooLarge\"}}" );
                sw.WriteLine($"{{\"event\":\"Bounty\",\"timestamp\":\"{day1:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Beta\"}}" );
            }
            using (var sw = new StreamWriter(file2))
            {
                // Beta system, second day (Combat)
                // Add FSDJump to Beta at the start of day2 to ensure visit window
                sw.WriteLine($"{{\"event\":\"FSDJump\",\"timestamp\":\"{day2:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Beta\",\"JumpDist\":15.0,\"SystemAddress\":2,\"StarPos\":\"[1,1,1]\",\"SystemAllegiance\":\"Empire\",\"SystemEconomy\":\"Agriculture\",\"SystemEconomy_Localised\":\"Agriculture\",\"SystemSecondEconomy\":\"None\",\"SystemSecondEconomy_Localised\":\"None\",\"SystemGovernment\":\"Dictatorship\",\"SystemGovernment_Localised\":\"Dictatorship\",\"SystemSecurity\":\"Low\",\"SystemSecurity_Localised\":\"Low\",\"Population\":2000,\"Body\":\"Beta 1\",\"BodyID\":2,\"BodyType\":\"Star\",\"FuelUsed\":2.0,\"FuelLevel\":20.0,\"event_timestamp\":\"{day2:yyyy-MM-ddTHH:mm:ss}\"}}" );
                sw.WriteLine($"{{\"event\":\"Bounty\",\"timestamp\":\"{day2:yyyy-MM-ddTHH:mm:ss}\",\"StarSystem\":\"Beta\"}}" );
            }
            return tempDir;
        }

        [Fact(Timeout = 20000)]
        public async Task BySystemGrouping_WithCategoryFilter_OnlyShowsMatchingCategory()
        {
            await Task.Run(() => {
                var tempDir = CreateBySystemCategoryTestDir();
                var output = Path.Combine(Path.GetTempPath(), "cc_bysyscat_report.html");
                try
                {
                    // Only include 'Travel' category (should show FSDJump events)
                    var cliDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "CmdrsChronicle.Cli", "bin", "Debug", "net8.0"));
                    var csprojPath = Path.GetFullPath(Path.Combine(cliDir, "..", "..", "..", "CmdrsChronicle.Cli.csproj"));
                    var psi = new ProcessStartInfo
                    {
                        FileName = "dotnet",
                        Arguments = $"run --project \"{csprojPath}\" -- --input \"{tempDir}\" --output \"{output}\" --start {DateTime.Today.AddDays(-3):yyyy-MM-dd} --end {DateTime.Today:yyyy-MM-dd} --type by-system --category TravelAndNavigation",
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
                    // Should contain FSDJump (Travel) but not Bounty (Combat)
                    Assert.Contains("Alpha", html);
                    Assert.Contains("Beta", html);
                    Assert.DoesNotContain("Bounty", html); // Bounty is not in Travel category
                }
                finally { 
                    // try { 
                    //     // Directory.Delete(tempDir, true); 
                    // } catch { } 
                    // try { 
                    //     // File.Delete(output); 
                    // } catch { } 
                }
            });
        }

        [Fact(Timeout = 20000)]
        public async Task BySystemGrouping_WithDateAndCategoryFilters_OnlyShowsMatchingEvents()
        {
            await Task.Run(() => {
                var tempDir = CreateBySystemCategoryTestDir();
                var output = Path.Combine(Path.GetTempPath(), "cc_bysyscat_report2.html");
                try
                {
                    // Only include day2 and 'Combat' category (should only see Beta system with Bounty event)
                    var cliDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "CmdrsChronicle.Cli", "bin", "Debug", "net8.0"));
                    var csprojPath = Path.GetFullPath(Path.Combine(cliDir, "..", "..", "..", "CmdrsChronicle.Cli.csproj"));
                    var psi = new ProcessStartInfo
                    {
                        FileName = "dotnet",
                        Arguments = $"run --project \"{csprojPath}\" -- --input \"{tempDir}\" --output \"{output}\" --start {DateTime.Today.AddDays(-1):yyyy-MM-dd} --end {DateTime.Today:yyyy-MM-dd} --type by-system --category CombatShip",
                        WorkingDirectory = cliDir,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false
                    };
                    using var proc = Process.Start(psi);
                    proc.WaitForExit(15000);
                    var stdout = proc.StandardOutput.ReadToEnd();
                    var stderr = proc.StandardError.ReadToEnd();
                    // Always print CLI output for diagnostics
                    Console.WriteLine("[CLI STDOUT]\n" + stdout);
                    Console.WriteLine("[CLI STDERR]\n" + stderr);
                    Assert.True(proc.ExitCode == 0, $"CLI exited with code {proc.ExitCode}. StdErr: {stderr}");
                    Assert.True(File.Exists(output), "Output report file not found");
                    var html = File.ReadAllText(output);
                    Assert.Contains("Beta", html);
                    Assert.DoesNotContain("Alpha", html);
                    // The tile title is 'Most Hunted Ship Types', not 'Bounty', so check for the system name only
                    Assert.DoesNotContain("FSDJump", html); // FSDJump is not in Combat category
                }
                finally { 
                    // try { Directory.Delete(tempDir, true); } catch { } 
                    // try { File.Delete(output); } catch { } 
                }
            });
        }
    }
}
