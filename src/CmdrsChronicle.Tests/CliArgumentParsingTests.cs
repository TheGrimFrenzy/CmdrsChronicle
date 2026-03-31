using System.CommandLine;
using System.CommandLine;
using Xunit;
using System.Threading.Tasks;
using System.CommandLine.Invocation;
using System.CommandLine.Testing;

namespace CmdrsChronicle.Cli.Tests
{
    public class CliArgumentParsingTests
    {
        [Fact]
        public async Task HelpOption_PrintsHelp()
        {
            var rootCommand = Program.BuildRootCommand();
            var console = new TestConsole();
            var result = await rootCommand.InvokeAsync("--help", console);
            var output = console.Out.ToString();
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
            var console = new TestConsole();
            var args = new[]
            {
                "--input", "C:/logs",
                "--output", "report.html",
                "--start", "2026-03-01",
                "--end", "2026-03-07",
                "--type", "by-system",
                "--category", "mining|trade",
                "--style", "colorful",
                "--max-parallelism", "4"
            };
            var result = await rootCommand.InvokeAsync(args, console);
            // No exception = success
            Assert.Equal(0, result);
        }
    }
}
