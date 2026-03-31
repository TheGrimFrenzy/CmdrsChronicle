using System.CommandLine;
using System;
using Xunit;
using System.Threading.Tasks;
using System.CommandLine.Invocation;

namespace CmdrsChronicle.Cli.Tests
{
    public class CliArgumentParsingTests
    {
        [Fact]
        public async Task HelpOption_PrintsHelp()
        {
            var rootCommand = Program.BuildRootCommand();
            var sw = new System.IO.StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(sw);
            try
            {
                var result = rootCommand.Invoke("--help");
            }
            finally
            {
                Console.SetOut(originalOut);
            }
            var output = sw.ToString();
            Assert.Contains("CmdrsChronicle CLI", output);
            Assert.Contains("--start", output);
            Assert.Contains("--end", output);
            Assert.Contains("--style", output);
            Assert.Contains("--type", output);
            Assert.Contains("--category", output);
        }

        [Fact]
        public async Task ArgumentParsing_ParsesAllOptions()
        {
            var rootCommand = Program.BuildRootCommand();
            var sw = new System.IO.StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(sw);
            // Create and use a temporary input directory so the test doesn't require a fixed path
            var tempInput = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "CmdrsChronicleTestInput_" + System.Guid.NewGuid().ToString("N"));
            System.IO.Directory.CreateDirectory(tempInput);
            var args = new[]
            {
                "--input", tempInput,
                "--output", "report.html",
                "--start", "2026-03-01",
                "--end", "2026-03-07",
                "--type", "by-system",
                "--category", "mining|trade",
                "--style", "colorful",
                "--max-parallelism", "4"
            };
            try
            {
                var result = rootCommand.Invoke(args);
                // No exception = success
                Assert.Equal(0, result);
            }
            finally
            {
                Console.SetOut(originalOut);
                try { System.IO.Directory.Delete(tempInput, true); } catch { }
            }
        }
    }
}
