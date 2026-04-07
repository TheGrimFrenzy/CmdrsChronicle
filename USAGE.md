# CmdrsChronicle Usage Guide

CmdrsChronicle is a CLI tool that generates self-contained HTML reports from Elite Dangerous journal log files.

- See [specs/001-cmdrs-chronicle/spec.md](specs/001-cmdrs-chronicle/spec.md) for the full specification.
- See [README.md](README.md) for project overview and setup.

---

## Installation

1. Download the latest ZIP release and extract it to a folder of your choice.
2. Ensure you have the [.NET 8 Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) installed.
3. Run from the extracted folder:
   ```
   CmdrsChronicle.Cli.exe --help
   ```

---

## Quick Start

Generate a report for the past week using your default Elite Dangerous journal directory:

```
CmdrsChronicle.Cli.exe --start 2026-03-29 --end 2026-04-05
```

The output HTML file will be written to your current directory and printed to the console.

---

## All Options

| Option | Default | Description |
|---|---|---|
| `--input <path>` | Auto-detected ED journal folder | Path to the directory containing journal log files |
| `--output <path>` | `CmdrsChronicle_<start>-<end>.html` | Path to write the generated HTML report |
| `--start <yyyy-MM-dd>` | (none) | Start date for filtering events |
| `--end <yyyy-MM-dd>` | (none) | End date for filtering events |
| `--type <summary\|by-system>` | `summary` | Report type: overall summary or grouped by star system |
| `--style <elegant\|colorful\|galnet>` | `elegant` | Visual style of the report |
| `--category <name>` | (all) | Restrict infographics to one or more categories (pipe-separated) |
| `--sort <categories>` | (none) | Comma-separated category order for infographic sections |
| `--max-parallelism <n>` | CPU count | Max number of journal files to parse in parallel |
| `--silent` | false | Suppress all output except the final report path |
| `--interactive` | false | Launch interactive mode to review and edit options before generating |

---

## Usage Examples

### Generate a weekly summary report
```
CmdrsChronicle.Cli.exe --start 2026-03-29 --end 2026-04-05
```

### Generate a report with a specific output path
```
CmdrsChronicle.Cli.exe --start 2026-03-29 --end 2026-04-05 --output C:\Reports\weekly.html
```

### Use a custom journal directory
```
CmdrsChronicle.Cli.exe --input "D:\Games\Elite\Journals" --start 2026-03-29 --end 2026-04-05
```

### Use the colorful style
```
CmdrsChronicle.Cli.exe --start 2026-03-29 --end 2026-04-05 --style colorful
```

### Use the GalNet-style report
```
CmdrsChronicle.Cli.exe --start 2026-03-29 --end 2026-04-05 --style galnet
```

### Group report by visited star system
```
CmdrsChronicle.Cli.exe --start 2026-03-29 --end 2026-04-05 --type by-system
```

### Show only travel and exploration infographics
```
CmdrsChronicle.Cli.exe --start 2026-03-29 --end 2026-04-05 --category TravelAndNavigation|Exploration
```

### By-system report filtered to combat only
```
CmdrsChronicle.Cli.exe --start 2026-03-29 --end 2026-04-05 --type by-system --category CombatShip
```

### Suppress console output (for scripting)
```
CmdrsChronicle.Cli.exe --start 2026-03-29 --end 2026-04-05 --silent
```
The output path is printed as the only line on stdout when `--silent` is used.

### Limit parallel journal parsing (lower resource use)
```
CmdrsChronicle.Cli.exe --start 2026-03-29 --end 2026-04-05 --max-parallelism 2
```

---

## Infographic Categories

You can filter to one or more of these categories using `--category`:

- `TravelAndNavigation`
- `Exploration`
- `Exobiology`
- `CombatShip`
- `CombatOnFoot`
- `ThargoidAX`
- `Trade`
- `Mining`
- `Missions`
- `EngineeringAndSynthesis`
- `FleetCarrierOperations`
- `Powerplay`
- `SocialMulticrew`
- `CodexAndDiscoveries`
- `SettlementActivities`
- `CrimeAndSecurity`
- `EconomyAndMarket`
- `ShipManagementAndOutfitting`
- `MaterialGathering`
- `SalvageAndRecovery`
- `PassengerTransport`
- `Administrivia`

Separate multiple categories with a pipe character: `--category TravelAndNavigation|Exploration`

---

## Troubleshooting

### No report is generated / "nothing to report" page appears
- Check that the `--start` and `--end` dates include the dates you played.
- Verify your `--input` directory contains files matching `Journal.<date>T<time>.<nn>.log`.
- Journal files from older or modded installs may not match the required filename format and will be skipped.

### The report is missing events I expected
- Parse errors for individual JSON lines are silently skipped and recorded in an HTML comment at the top of the output file. Open the HTML file in a text editor and search for `<!-- Parse errors` to review them.
- Confirm the events fall within the `--start` to `--end` date range (both ends inclusive).

### The CLI exits with a non-zero code
- Exit code 1 indicates an unrecoverable error such as a missing input directory, missing schema, or missing templates.
- Check that the `--input` path exists and contains readable `.log` files.
- Ensure the `templates/` and `infographics/` directories are present alongside the executable (they are included in the ZIP release).

### Performance is slow
- Use `--max-parallelism` to control how many files are parsed in parallel. A higher value speeds up parsing on multi-core systems.
- Alternatively, set the environment variable `CMDRSCHRONICLE_MAX_PARALLELISM` to configure the default without specifying it each run.

### Output HTML won't open in the browser
- The output is a fully self-contained HTML file (all CSS and JS is inlined). It can be opened in any modern browser directly from disk.
- If your system doesn't open HTML files by default, right-click and choose "Open with" your preferred browser.

### Customising infographics
- Each infographic tile is defined by a JSON file in the `infographics/` directory.
- To add or modify a tile, edit or create the corresponding JSON file in the appropriate category subdirectory.
- No recompilation is required — changes are picked up the next time the CLI runs.

