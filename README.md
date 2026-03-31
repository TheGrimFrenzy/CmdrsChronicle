# CmdrsChronicle

A .NET tool to generate self-contained HTML reports from Elite Dangerous logs, with infographic support and robust CLI.

## Project Structure

```
/ (repo root)
│
├── .gitignore
├── README.md
├── CmdrsChronicle.sln
│
├── src/
│   ├── CmdrsChronicle.Core/
│   ├── CmdrsChronicle.Cli/
│   ├── CmdrsChronicle.Infographics/
│   ├── CmdrsChronicle.Tests/
│   └── Installer/
│
└── tests/
    ├── unit/
    ├── integration/
    └── contract/
```

- **src/**: Production code, separated by concern.
- **tests/**: Test code, separated by type.
- **.gitignore**: Prevents build artifacts and secrets from being committed.
- **CmdrsChronicle.sln**: Solution file for managing all projects.

See specs/001-cmdrs-chronicle/plan.md for more details.

## Test Directory Layout

The `tests/` directory is organized for clarity and CI configuration:

- `tests/unit/` — Unit tests for individual methods/classes (fast, isolated)
- `tests/integration/` — Integration tests for cross-component or system-level scenarios
- `tests/contract/` — Contract/API tests for public interfaces (e.g., CLI, file formats)

> **Best Practice:** Place new test files in the appropriate subdirectory. Use xUnit for all test projects. See `src/CmdrsChronicle.Tests` for the main test project scaffold.

## Taglines File

The report masthead displays a random tagline from `templates/taglines.txt`.

- Each line is a tagline.
- Lines starting with `#` are comments and ignored.
- At least 5 non-comment taglines are required for MVP.

**Example:**
```
# CmdrsChronicle Taglines
To boldly go where no pilot has gone before.
The galaxy is vast—make your mark.
Every jump tells a story.
From humble Sidewinder to legend.
Charting the unknown, one log at a time.
```

> To add or edit taglines, simply update this file. No code changes required.

## Infographics Directory Structure

The `/infographics` directory contains a subdirectory for each infographic category. Each subdirectory holds JSON definitions for infographics in that category.

**Categories (as of spec):**
- TravelAndNavigation
- Exploration
- Exobiology
- CombatShip
- CombatOnFoot
- ThargoidAX
- Trade
- Mining
- Missions
- EngineeringAndSynthesis
- FleetCarrierOperations
- Powerplay
- SocialMulticrew
- CodexAndDiscoveries
- SettlementActivities
- CrimeAndSecurity
- EconomyAndMarket
- ShipManagementAndOutfitting
- MaterialGathering
- SalvageAndRecovery
- PassengerTransport
- Administrivia

> To add a new infographic, place its JSON definition in the appropriate category subdirectory. Categories are defined in the project specification.

## .NET SDK and Language Version

- All projects target **.NET 8 (net8.0)** and **C# 12**.
- The required SDK version is pinned in `global.json` (8.0.100).
- Language version is set in `Directory.Build.props` for consistency.

> To build and run this project, install the .NET 8 SDK (8.0.100 or later).
