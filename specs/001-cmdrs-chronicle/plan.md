
# Requirement-to-Section Mapping
| Requirement/User Story         | Section(s)      |
|-------------------------------|-----------------|
| Tagline selection             | 1.1             |
| Infographics directory        | 1.2             |
| No data message logic         | 1.3             |
| Parallel file parsing         | 2.1, 2.2        |
| SQLite event storage          | 2.3             |
| CLI contract                  | 2.4             |
| Installer packaging           | 3.1             |
| End-user instructions         | 3.2             |
| Performance/memory NFRs       | 4.1             |

# Glossary
- **Infographic tile**: A single visual element in the report, rendered from a JSON definition.
- **Infographic category**: A grouping of related infographic tiles (e.g., Travel, Combat).
- **No data message**: A contextually relevant message shown when no valid events exist for the report timeframe.
- **Tagline**: A short, random phrase displayed in the report masthead.
- **MVP**: Minimum Viable Product; the smallest set of features that delivers user value.
- **NFR**: Non-Functional Requirement (e.g., performance, memory usage).

# 1. Core Features Implementation

## 1.1 Report Tagline Implementation

The report masthead displays a tagline, randomly selected from templates/taglines.txt. This file contains one tagline per line, with lines starting with # treated as comments and ignored. On each report generation, a random non-comment line is chosen and inserted as the masthead tagline.

**Acceptance Criteria:**
- Tagline is randomly selected from non-comment lines
- File format and location documented in README

## 1.2 Infographics Directory Structure

As part of project setup, generate an /infographics directory at the project root. For each identified infographic category, create a subdirectory under /infographics (e.g., /infographics/Travel, /infographics/Combat, etc.).

**Acceptance Criteria:**
- Directory and subdirectories exist for all categories
- Structure documented in README

As part of project setup, generate an /infographics directory at the project root. For each identified infographic category (see spec.md), create a subdirectory under /infographics (e.g., /infographics/Travel, /infographics/Combat, etc.). This structure supports modular infographic definition and theming. Document the directory structure in the README for onboarding and future contributors.

## 1.3 No Data Message Implementation


When generating a report with no valid events, load the appropriate message from templates/no-data-messages.json using the current ordinal day (1-365). For leap years, after Feb 29, subtract 1 from the ordinal day to align with the message/holiday entries. Interpolate {cmdrName}, {lastSystem}, and {lastDate} into the message. To populate {lastSystem} and {lastDate}, read the most recent journal file prior to the report's begin date and extract the last known system and timestamp. If the day is missing, use a default or random fallback message. This logic ensures non-repetitive, contextually relevant 'no data' content year-round.

**Acceptance Criteria:**
- No data message is selected by ordinal day, leap year logic correct
- Placeholders interpolated
- Fallback/default message logic works

# 2. Data Processing & Layout Implementation

**Branch**: `[###-feature-name]` | **Date**: [DATE] | **Spec**: [link]
**Input**: Feature specification from `/specs/[###-feature-name]/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.


## 2.1 Parallel File Parsing


Journal files are discovered and parsed in parallel using the Task Parallel Library (TPL) with a concurrency cap, as defined in spec.md §2.1. The concurrency cap defaults to Environment.ProcessorCount but can be configured via CLI argument (--max-parallelism), environment variable (CMDRSCHRONICLE_MAX_PARALLELISM), or config file. Directory scan and filename validation strictly follow the canonical pattern in research-naming.md. Only files matching the pattern `Journal.<YYYY-MM-DDThhmmss>.<nn>.log` are processed; all others are ignored and logged as skipped. Invalid or unreadable files are logged as errors in a standardized format.

All parallel/concurrent logic for file reading/parsing MUST be developed test-first, with unit and integration tests written before implementation. CI MUST enforce green tests for all parallel code paths. All new logic MUST include code comments and documentation explaining the concurrency cap, error handling, and file validation logic, in accordance with the project constitution.

**Acceptance Criteria:**
- File discovery, parallel parsing, and concurrency cap logic are covered by unit and integration tests (test-first, enforced by CI)
- Directory scan and filename validation strictly follow research-naming.md; only canonical files are processed
- Invalid, unreadable, or non-canonical files are skipped and logged as errors in a standardized format
- Concurrency cap is configurable via CLI, environment variable, or config file; default is Environment.ProcessorCount
- All new logic is documented with code comments and referenced in the README (rationale for parallelism, concurrency cap, and error handling)
- All requirements above are cross-referenced in spec.md and tasks.md to avoid duplication

## 2.2 Parallel Infographic Rendering

Infographic tiles are rendered from /infographics JSON files by category using data from SQLite DB. Rendering is performed in parallel (one task per tile) using TPL with concurrency cap.

**Acceptance Criteria:**
- Infographic tiles rendered in parallel
- Concurrency cap logic tested

CmdrsChronicle is a Windows console application that processes Elite Dangerous journal logs and generates a fully self-contained HTML report. The tool is implemented in C# 12 targeting .NET 8 (LTS), published as a single-file, self-contained executable requiring no external interpreters or VMs. The architecture is library-first: all core logic (parsing, aggregation, report generation) resides in reusable libraries, with a thin CLI entry point. Data is processed in-memory, and the output is a single HTML file with embedded assets. The CLI supports flexible filtering, grouping, and style options, and robust error handling for empty or malformed data. Testing is enforced via xUnit and CI. No persistent storage or server components are required.


## 2.3 SQLite Event Storage


All event schemas are mapped to tables in an in-memory SQLite database. **Table schemas are generated once from the canonical event schemas as a static SQL file (or C# migration), which is committed to the repository and used to initialize the in-memory DB at runtime.** Reserved word handling is enforced as per plan.md and data-model.md. Integration and unit tests are written before implementation (test-first) and verify schema mapping and data insertion. If upstream schemas change, the static SQL file is regenerated and recommitted.

**Acceptance Criteria:**
- Static SQL schema file is generated from all canonical event schemas and committed to the repo
- In-memory SQLite DB is initialized from the committed static SQL schema for each run
- Reserved word handling logic matches plan.md and spec.md
- Integration and unit tests verify schema mapping and data insertion (test-first)
- Process for updating static SQL when upstream schemas change is documented

## 2.4 CLI Contract

The CLI supports all options from contracts/cli.md, with robust argument parsing and detailed --help output. Tests cover argument parsing and help.

**Acceptance Criteria:**
- Argument parsing supports all CLI options from contracts/cli.md
- --help output is detailed
- Tests cover argument parsing and help

**Event Schema Reference**: The authoritative schemas for Elite Dangerous journal events are available at https://github.com/jixxed/ed-journal-schemas/tree/main/schemas. All event parsing and validation should use these schemas to ensure compatibility and correctness.


# 3. Packaging & End-User Experience Implementation

## 3.1 Installer Packaging

The installer project packages the /infographics directory and all category subdirectories so they are delivered with the app. The application reads infographics from the installed directory, allowing end users to add or modify their own. Build produces MSIX or MSI. Installer instructions are included in README.

**Acceptance Criteria:**
- Installer project created
- Build produces MSIX or MSI
- Installer instructions in README

## 3.2 End-User Instructions

USAGE.md and README include clear end-user instructions. Infographics directory is packaged and customizable. Tests for installer build are present.

**Acceptance Criteria:**
- USAGE.md and README include clear end-user instructions
- Infographics directory packaged and customizable
- Tests for installer build

This project is being built by a single developer who is not an expert in C# or .NET. All code implementations and iterative changes should be accompanied by clear explanations of what is being done and why, to maximize maintainability and learning.

<!--
  ACTION REQUIRED: Replace the content in this section with the technical details
  for the project. The structure here is presented in advisory capacity to guide
  the iteration process.
-->


# 4. Non-Functional Requirements (NFRs) Implementation

## 4.1 Performance and Memory Usage

Performance test: report generation <30s for 1 week of logs. Memory test: <500MB RAM for 10,000 events. Test methodology/results documented in README/CONTRIBUTING.md. Tests run in CI and fail if constraints not met.

**NFR Reference:** See also spec.md and tasks.md for explicit performance/memory goals
**Primary Dependencies**: .NET SDK (System.CommandLine, System.Text.Json, System.IO, System.Linq), no external interpreters or VMs required
**Storage**: Embedded in-memory database (SQLite in-memory mode via .NET); supports complex queries for infographics; data is loaded from log files into the in-memory DB for the duration of report generation (no persistent DB)
**Reserved Word Handling**: All event property names that are reserved SQL keywords for the target DB (e.g., `timestamp`, `order`, `group`, etc.) must be prefixed with `event_` in generated table/column names (e.g., `event_timestamp`). This rule is documented in the data model and must be followed in all code generation and migrations. Maintain a list of reserved words for the chosen DB engine and apply this rule consistently.
**Testing**: xUnit (unit/integration), .NET built-in test runner
**Target Platform**: Windows 10+ (x64/x86), .NET 8 self-contained console app
**Project Type**: CLI tool (single executable), library-first structure, plus standard Windows installer (MSIX or MSI) as part of build pipeline
**Performance Goals**: Generate report for 1 week of logs (<30s typical), handle up to 10,000 events in memory
**Constraints**: No external runtime dependencies; must run on clean Windows install with .NET 8 self-contained publish; memory usage < 500MB for typical use
**Scale/Scope**: Single-user, local execution; not intended for multi-user or server scenarios

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Gates (from constitution v1.0.0):**
- Library-First: All logic must be in testable libraries, not just CLI glue.
- CLI Interface: Must provide a robust CLI with text I/O, support for automation.
- Test-First: Unit and contract tests must be written before implementation; CI must enforce green tests.
- Integration Testing: Cross-library and external integration tests required.
- Observability, Versioning & Simplicity: Structured logs, semantic versioning, and simple, well-documented solutions required.
- .NET stack: Must use .NET (C#), follow csproj/nuget conventions, and support CI for build/test/lint.
- Security: No secrets in repo; security-sensitive changes require threat reasoning.
- Workflow: All changes via PR, with tests and review.

## Project Structure
## Architectural Details

### File Discovery and Parsing
- Journal files are discovered in the target directory using a filename pattern (see research-naming.md).
- Files are read and parsed in parallel using the Task Parallel Library (TPL) with a concurrency cap. By default, MaxDegreeOfParallelism is set to Environment.ProcessorCount, but this can be overridden via a configuration file or CLI argument (see usage docs). This maximizes performance and avoids resource exhaustion.
- Use Parallel.ForEach or Task.Run for parallelism; avoid creating raw threads. Document concurrency cap logic in code and README.
- All parallel/concurrent logic for file reading/parsing must be developed test-first, with unit and integration tests written before implementation. CI must enforce green tests for all parallel code paths.
- Each file is parsed line-by-line as JSON, skipping invalid lines.
- Errors are logged for diagnostics.

### Infographic Rendering
- Infographics are generated from /infographics JSON files using aggregated data from the SQLite in-memory DB.
- Infographic rendering is performed in parallel (one task per infographic) using TPL with a concurrency cap. By default, MaxDegreeOfParallelism is set to Environment.ProcessorCount, but this can be overridden via a configuration file or CLI argument. This maximizes performance and avoids resource exhaustion.
- Use Parallel.ForEach or Task.Run for parallel infographic rendering; document concurrency cap logic in code and README.
- All parallel/concurrent logic for infographic rendering must be developed test-first, with unit and integration tests written before implementation. CI must enforce green tests for all parallel code paths.
- Infographic rendering is modular and extensible.

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)
```text
src/
├── CmdrsChronicle.Core/        # Core library: parsing, aggregation, data model
├── CmdrsChronicle.Cli/         # CLI entry point (Program.cs)
├── CmdrsChronicle.Infographics/ # Infographic rendering logic (optional split)
├── CmdrsChronicle.Tests/       # xUnit test project (unit/integration)
├── Installer/                  # Installer project (MSIX or MSI packaging)

tests/                         # (if additional test projects needed)
├── contract/
├── integration/
└── unit/
```
│   ├── components/
│   ├── pages/
│   └── services/
└── tests/

# [REMOVE IF UNUSED] Option 3: Mobile + API (when "iOS/Android" detected)
api/
└── [same as backend above]

ios/ or android/
└── [platform-specific structure: feature modules, UI flows, platform tests]
```

**Structure Decision**: [Document the selected structure and reference the real
directories captured above]

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |
