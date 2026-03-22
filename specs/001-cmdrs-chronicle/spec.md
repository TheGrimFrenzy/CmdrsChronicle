
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

# 1. Core Features

## 1.1 Report Tagline Logic

The report masthead includes a tagline, randomly selected from templates/taglines.txt. This file contains one tagline per line; lines starting with # are comments and ignored. Each time a report is generated, a random non-comment line is chosen as the tagline for the masthead.

**Acceptance Criteria:**
- Tagline is randomly selected from non-comment lines
- File format and location documented in README

## 1.2 Infographics Directory Structure

As part of project setup, generate an /infographics directory at the project root. For each identified infographic category, create a subdirectory under /infographics (e.g., /infographics/Travel, /infographics/Combat, etc.).

**Acceptance Criteria:**
- Directory and subdirectories exist for all categories
- Structure documented in README

## 1.3 No Data Message Logic


When no valid events exist for the report timeframe, the report generator selects a 'no data' message from templates/no-data-messages.json based on the current ordinal day of the year (1-365). For leap years, after Feb 29, subtract 1 from the ordinal day to align with the message/holiday entries (i.e., Mar 1 is day 60, Mar 2 is day 61, etc.).

The selected message supports string interpolation for {cmdrName}, {lastSystem}, and {lastDate}. To populate {lastSystem} and {lastDate}, the application must read the most recent journal file prior to the report's begin date and extract the last known system and timestamp. If no message exists for the day, a default or random fallback message is used. This ensures variety and relevance in 'no data' reports throughout the year.

**Acceptance Criteria:**
- No data message is selected by ordinal day, leap year logic correct
- Placeholders interpolated
- Fallback/default message logic works


# 2. Report Layout & Data Processing


## 2.1 Parallel File Parsing

Journal files are discovered and parsed in parallel using Task Parallel Library (TPL) with a concurrency cap. Directory scan and filename validation follow the pattern in research-naming.md. Parallelism uses TPL with MaxDegreeOfParallelism set to Environment.ProcessorCount or a configurable value.

**Acceptance Criteria:**
- File discovery, parallel parsing, and concurrency cap logic tested
- Directory scan and filename validation as per research-naming.md
- Parallelism uses TPL with concurrency cap

## 2.2 Parallel Infographic Rendering

Infographic tiles are rendered from /infographics JSON files by category using data from SQLite DB. Rendering is performed in parallel (one task per tile) using TPL with concurrency cap.

**Acceptance Criteria:**
- Infographic tiles rendered in parallel
- Concurrency cap logic tested


## 2.3 SQLite Event Storage

All event schemas are mapped to tables in an in-memory SQLite database. Reserved word handling is enforced as per plan.md and data-model.md. Integration tests verify schema mapping and data insertion.

**Acceptance Criteria:**
- All event schemas mapped to tables as per data-model.md
- Reserved word handling logic matches plan.md and spec.md
- Integration tests verify schema mapping and data insertion


## 2.4 CLI Contract

The CLI supports all options from contracts/cli.md, with robust argument parsing and detailed --help output. Tests cover argument parsing and help.

**Acceptance Criteria:**
- Argument parsing supports all CLI options from contracts/cli.md
- --help output is detailed
- Tests cover argument parsing and help

The "elegant" format emphasizes clarity, whitespace, and subtle color accents. It uses a modern, readable font, soft background gradients, and card-based sections for each major report area. Infographics are rendered as visually distinct tiles with gentle drop shadows and minimalist icons.

### Structure

- **Header**: Contains the report title, date generated, and Commander name (if available). Uses a large, bold font and a subtle background gradient.
- **Summary Section**: Displays key statistics (events processed, duration, top categories) in a horizontal card layout.
- **Timeline**: Chronological list of major events, grouped by day or system (depending on report type). Each event is shown in a bordered card with timestamp, event type, and summary.
- **Infographics Grid**: Infographics defined in `/infographics` are rendered as tiles in a responsive grid. Each tile includes:
  - Title (from JSON)
  - Icon or chart (per type)
  - Key value or chart visualization
  - Subtle hover effect and drop shadow
- **Per-System Sections** (if `by-system`): Each system gets a collapsible section with its own timeline and infographics.
- **Diagnostics/Log Area**: Parsing warnings, file scan results, and any errors are shown in a collapsible panel at the bottom.
- **Footer**: Provenance info (source directory, date generated, tool version), and a small logo or signature.

### Visual Style

- **Font**: Uses a sans-serif font stack (e.g., `Segoe UI`, `Roboto`, `Arial`, `sans-serif`).
- **Colors**: Soft background gradient (e.g., #f8fafc to #e9ecef), primary accent color for headers and cards (e.g., #3a506b), and muted grays for secondary text.
- **Cards/Tiles**: Rounded corners, light drop shadows, and subtle borders. Infographic tiles use a slightly raised effect on hover.
- **Charts**: Minimalist, with clean axes and no gridlines unless necessary. Colors are chosen for clarity and accessibility.
- **Responsiveness**: Layout adapts to mobile and desktop, with infographics stacking vertically on narrow screens.

### Infographic Tile Example (Elegant)

```
<div class="infographic-tile elegant">
  <div class="infographic-title">Top Commodity Sold</div>
  <div class="infographic-icon"><!-- SVG or icon here --></div>
  <div class="infographic-value">Palladium</div>
  <div class="infographic-subtext">42 units</div>
</div>
```

CSS classes and structure are designed for easy theming; the `elegant` class is applied to all elements when this style is selected.



# 3. Packaging & End-User Experience

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

This section describes the structure, layout, and visual style of the generated HTML report when the `colorful` style is selected, based on the provided example.

### Overview

The "colorful" format uses vibrant, multi-color gradients, bold metric badges, and visually striking infographic tiles. It is designed for maximum visual impact, with each category and section using distinct color themes. The layout is CSS2-compatible (floats, display:table) for broad rendering support.


# 4. Non-Functional Requirements (NFRs)

## 4.1 Performance and Memory Usage

Performance test: report generation <30s for 1 week of logs. Memory test: <500MB RAM for 10,000 events. Test methodology/results documented in README/CONTRIBUTING.md. Tests run in CI and fail if constraints not met.

**NFR Reference:** See also plan.md and tasks.md for explicit performance/memory goals

- **Masthead/Header**: Large, gradient background with the report title, tagline, and date strip. Title uses uppercase, heavy font with a drop shadow. Includes thick and thin colored rules for separation.
- **Summary/Key Metrics**: Prominent badges or metric strips, often with vertical or rotated text, using high-contrast colors and gradients.
- **Timeline**: Alternating left/right tile layout for major events, each tile containing:
  - Icon area (60px wide)
  - Main tile (420px+ wide), with gradient background and drop shadow
  - Badge section (vertical, with rotated text and unit)
  - Content area (title, stat, details)
  - Tiles alternate left/right for visual rhythm
- **Infographics**: Rendered as large, colorful tiles with:
  - Gradient backgrounds themed by category (e.g., travel, combat, trade)
  - Title bar, main stat, and detail area
  - Bar charts, tables, or icons as needed
  - Table and bar chart elements use white/contrasting text for readability
- **Category Gradients**: Each major category (travel, combat, exploration, etc.) has a unique gradient background for its tiles, e.g.:
  - `.grad-travel` (blue/purple), `.grad-combat` (red/pink), `.grad-trade` (orange/yellow), etc.
- **Diagnostics/Log Area**: Styled with colored borders and backgrounds for visibility.
- **Footer**: Uses color strips or gradients, with provenance info and optional logo.

### Visual Style

- **Font**: Sans-serif stack (e.g., `Segoe UI`, Tahoma, Geneva, sans-serif), with heavy weights for titles and badges.
- **Colors**: Intense gradients (linear-gradient backgrounds), high-contrast text, and colored badge strips. Each section/category uses a distinct palette.
- **Tiles**: Large, rounded corners, strong drop shadows, and alternating left/right alignment. Badges use vertical text and bold units.
- **Charts/Tables**: Minimalist, with white or near-white text on colored backgrounds. Bar charts use semi-transparent overlays for bars.
- **Responsiveness**: Fixed-width layout for desktop, with print/PDF compatibility (page-break avoidance on tiles).

### Infographic Tile Example (Colorful)

```
<div class="left-tile-wrapper">
  <div class="left-icon"><!-- Icon or SVG --></div>
  <div class="left-tile grad-travel">
    <div class="left-badge-section">
      <span class="left-badge">42</span>
      <span class="left-badge-unit">Jumps</span>
    </div>
    <div class="left-content">
      <div class="infographic-title">Longest Jump Streak</div>
      <div class="tile-stat">42</div>
      <div class="infographic-detail">Travelled 1,200 LY</div>
    </div>
  </div>
</div>
```

Category-specific gradients are applied via classes like `grad-travel`, `grad-combat`, etc. Tiles and badges use strong color contrast and bold typography for maximum impact.

---
# Feature Specification: CmdrsChronicle — Self-contained HTML report from Elite Dangerous logs

**Feature Branch**: `001-cmdrs-chronicle`  
**Created**: 2026-03-21  
**Status**: Draft  
**Input**: User description: "Build an application called CmdrsChronicle that will read Elite Dangerous log files and generate a self-contained HTML file. The user will run the application (optionally specifying a log file directory — otherwise defaulting to %USERPROFILE%\Saved Games\Frontier Developments\Elite Dangerous\ — as well as optional begin and end dates, otherwise defaulting to the last week), a style (elegant or colorful), a type (overall or by-system) optional category filter and optional category sort order.  The app will read the appropriate log files, storing the data in local in-memory database, and then query that database to produce a report (based on html and css templates driven by the style option) of the last weeks adventures, based on the json files it finds in an /infographics subdirectory, each of which defines an infographic."

## User Scenarios & Testing *(mandatory)*

<!--
  IMPORTANT: User stories should be PRIORITIZED as user journeys ordered by importance.
  Each user story/journey must be INDEPENDENTLY TESTABLE - meaning if you implement just ONE of them,
  you should still have a viable MVP (Minimum Viable Product) that delivers value.
  
  Assign priorities (P1, P2, P3, etc.) to each story, where P1 is the most critical.
  Think of each story as a standalone slice of functionality that can be:
  - Developed independently
  - Tested independently
  - Deployed independently
  - Demonstrated to users independently
-->

### User Story 1 - Generate a weekly summary report (Priority: P1)

As a player, I want to run CmdrsChronicle with no arguments and receive a self-contained HTML report summarizing my last week's activity so I can review highlights and share a single file with others.

**Why this priority**: Delivers immediate value with zero configuration and validates core parsing, aggregation, and templating flows.

**Independent Test**: Run the application without arguments on a machine containing at least one week of Elite Dangerous journal files and verify a single HTML file is produced and opens in a browser.

**Acceptance Scenarios**:

1. **Given** a user machine with journal files under the default Saved Games directory, **When** the user runs the application with no arguments, **Then** the tool produces `cmdrs-chronicle-<date-generated>.html` that contains a summary section, timeline of events, and embedded infographics defined by JSON files in `/infographics` (if present).
3. **Given** no journal files exist in the specified or default timeframe, **When** the user runs the application, **Then** the tool produces a special "nothing of interest happened" HTML report (selected from a configurable set of pre-written messages) that includes the usual header, summary metadata, and footer; before generating the report the tool will search for the most recent journal file with events prior to the requested `--start` date (or the most recent overall if `--start` is not provided), extract the last valid event to determine the Commander's name and the last system visited, and include those details in the report when available. The CLI returns exit code `0` indicating a report was generated.
2. **Given** malformed or partial journal files, **When** the user runs the tool, **Then** parsing errors are recorded in a non-fatal log area in the generated report and the process exits with a success code if at least one valid event was processed.

---

### User Story 2 - Generate a report filtered by system (Priority: P2)

As a player, I want to produce a report that groups and emphasizes events by star systems so I can review colonization, mining or mission activity for a specific world.

**Why this priority**: Common player workflow for trip reports and system-level activity summaries.

**Independent Test**: Run the application with `--type by-system` (or equivalent UI) and verify report sections are grouped by system and that grouping and ordering reflect system boundaries. To narrow results to a single system, combine `--type by-system` with existing filters such as `--category` or explicit `--start`/`--end` dates.

**Acceptance Scenarios**:

1. **Given** a set of journal files containing events with system context, **When** the user requests `by system` type (optionally combined with filters such as `--category` or a date range), **Then** the report contains a top-level system index and per-system sections showing events and infographics limited to the selected filters.

---

### User Story 3 - Custom date range, style, and category filtering (Priority: P3)

As a player, I want to generate a report for an arbitrary date range, choose a visual `style` (e.g., `elegant` or `colorful`), and optionally filter and order categories (for example `--category "mining|travel|trade"`) so I can create focused reports.

**Why this priority**: Enables power users and allows verification of filtering/sorting logic.

**Independent Test**: Run the application with explicit `--start` and `--end` dates, `--style colorful`, and `--category "mining|travel|trade"` and validate the produced report contains only events within the date range, uses the colorful visual palette, and includes categories filtered to `mining`, `travel`, `trade` in that order.

**Acceptance Scenarios**:

1. **Given** journal files covering multiple months, **When** the user specifies a narrow date window and category filter, **Then** the report only includes events in that window and only category entries matching the filter; infographics are rendered according to style selection.

---

### Edge Cases

- No journal files found in the specified or default timeframe: the tool produces a special "nothing of interest happened" report chosen from a configurable list of pre-written messages, including the standard header and footer; the tool will attempt to locate the most recent prior journal before the requested `--start` date to extract Commander name and last-seen system for inclusion in the report. If no prior journal exists, those fields will be marked `unknown`. The CLI returns exit code `0` indicating successful report generation.
- Partially corrupted journal files: the tool skips invalid lines, counts and reports parsing failures in the generated report, and still produces output if at least one valid event exists.
- Conflicting infographics: if multiple infographic JSON files define the same id, the tool prefers the most recently modified file and reports the conflict in the report footer.
- Very large datasets: the tool provides a progress summary and will abort with a clear message if memory usage exceeds a safe threshold (configurable), to avoid crashing the host.


## Requirements *(mandatory)*

<!--
  ACTION REQUIRED: The content in this section represents placeholders.
  Fill them out with the right functional requirements.
-->

### Functional Requirements

- **FR-001**: The tool MUST accept command-line options for: `--dir` (log directory), `--start` (start date), `--end` (end date), `--style` (elegant|colorful), `--type` (overall|by-system), and `--category` (optional filter + order). Defaults: `--dir` → `%USERPROFILE%\Saved Games\Frontier Developments\Elite Dangerous\`, `--start`/`--end` → last 7 days when not provided.
- **FR-002**: The tool MUST discover and read all journal files within the specified directory that match typical Elite Dangerous journal filename patterns and parse each line as JSON events according to the game's journal schema.
- **FR-003**: The tool MUST load any JSON files present in an `/infographics` subdirectory and use them to drive infographic rendering; each infographic JSON provides an `id`, display `title`, a `type` (chart, counter, timeline snippet), and a data-binding expression mapping parsed events to infographic values.
- **FR-004**: The tool MUST store parsed events in a transient, in-memory datastore to support ad-hoc queries, grouping, and aggregation during the report generation run. No external persistent database is required.
- **FR-005**: The tool MUST render a single self-contained HTML file containing all CSS, JavaScript, images, and serialized data required to view the report offline in a modern browser.
- **FR-006**: The tool MUST support two visual styles (`elegant`, `colorful`) selectable via `--style`; templates for both styles MUST be provided and applied at generation time.
- **FR-007**: The tool MUST allow category-based filtering and ordering via the `--category` argument. `--category` accepts a pipe-delimited list (e.g., `mining|travel|trade`) which (a) filters out any categories not present in the list and (b) defines the display order of categories in the generated report. If `--category` is omitted, all categories are included.
- **FR-008**: The tool MUST produce clear, actionable logging inside the generated report (parsing warnings, files scanned, counts of events processed). If at least one valid event is processed the tool returns exit code `0`. If no events are found in the specified timeframe the tool still returns exit code `0` but generates a special "nothing of interest happened" report (selected from a configurable list of pre-written options). In that case the tool SHOULD locate the most recent journal prior to the requested `--start` date (or the most recent overall if `--start` is not provided), extract the last valid event to determine Commander name and last system seen, and include those details in the report when available. The tool returns a non-zero exit code only for unrecoverable errors.
- **FR-009**: The tool MUST include an embedded summary section in the HTML with totals (events processed, duration covered, top categories) and a human-readable file-level provenance footer (source directory, date generated).



## Key Entities *(include if feature involves data)*

- **LogFile**: Represents a single journal/log file. Attributes: `path`, `modifiedAt`, `size`, and `lineCount`.
- **LogEvent**: A parsed journal line representing a game event. Attributes: `timestamp`, `eventType`, `payload` (raw JSON), and optional contextual attributes (system, body, planet, commander).
- **InfographicDefinition**: JSON document found in `/infographics`. Attributes:
  - `id` (string, required): Unique identifier for the infographic (filename without extension if not explicitly set).
  - `category` (string, required): Category for grouping (e.g., Mining, Combat).
  - `title` (string, required): Display title for the infographic tile.
  - `query` (string, required): SQL query or data-binding expression to compute the main value/statistic. Must use parameterized placeholders (e.g., `:startDate`, `:endDate`).
  - `threshold` (number, optional): Minimum value required to display the infographic (e.g., only show if count > 0).
  - `detailQuery` (string, optional): SQL query for breakdowns or chart data (e.g., bar chart by mineral type).
  - `chartType` (string, optional): Visualization type (e.g., `bar-chart`, `pie-chart`, `counter`).
  - `enabled` (boolean, optional): If false, infographic is ignored.
  - `presentationHints` (object, optional): Additional hints for rendering (e.g., color mapping, icon).

**Example InfographicDefinition JSON:**

```json
{
  "id": "asteroids-cracked",
  "category": "Mining",
  "title": "Asteroids Cracked",
  "query": "SELECT COUNT(*) AS count FROM asteroid_cracked WHERE event_timestamp >= :startDate AND event_timestamp < :endDate",
  "threshold": 1,
  "detailQuery": "SELECT body AS location, COUNT(*) AS cracked FROM asteroid_cracked WHERE event_timestamp >= :startDate AND event_timestamp < :endDate AND body IS NOT NULL GROUP BY body ORDER BY cracked DESC",
  "chartType": "bar-chart",
  "enabled": true
}
```

- **Validation Rules:**
- `query` and `detailQuery` must be valid SQL for the in-memory SQLite DB, using only whitelisted parameters (`:startDate`, `:endDate`, etc.).
- `id`, `category`, and `title` are required and must be unique within the `/infographics` directory.
- If `enabled` is false or missing, the infographic is ignored.
- If `threshold` is set, the infographic is only shown if the main value meets/exceeds it.
- `presentationHints` is reserved for future extensibility (e.g., custom colors, icons).

**Anticipated Infographic Categories:**
The following category names are recommended for the `category` attribute in each InfographicDefinition. These categories are used for grouping, filtering, and theming infographics in the report:

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

Each category corresponds to a major gameplay or reporting area. See internal documentation for mapping to event types.

- **Report**: The generated artifact. Attributes: `title`, `dateGenerated`, `style`, `sections` (collection of ReportSection), and `embeddedAssets`.
- **ReportSection**: A logical grouping inside the report (e.g., summary, per-system sections, timeline, infographics).


## Success Criteria *(mandatory)*

<!--
  ACTION REQUIRED: Define measurable success criteria.
  These must be technology-agnostic and measurable.
-->

### Measurable Outcomes

- **SC-001**: A user running the tool with default options on a machine with one week of journal files produces a valid, viewable single HTML output within 30 seconds (typical modern laptop); deviations must be documented and justified.
- **SC-002**: The produced HTML is fully self-contained (no external resource loading) and opens in Chrome, Edge, and Firefox without console errors related to missing assets.
- **SC-003**: When infographic JSON files are present, at least one infographic defined is rendered in the output report; if none can be rendered due to schema mismatch, the report contains a clear diagnostics section explaining why.
- **SC-004**: CLI exit codes: `0` when a report (including a "nothing of interest happened" report) is produced; non-zero only when an unrecoverable error occurred preventing report generation.

## Assumptions

- Journal files follow the standard Elite Dangerous JSON journal format (one JSON object per line). The tool will ignore non-JSON lines and report parsing failures.
- Infographic JSON files use a simple declarative schema: `{ "id": "<string>", "title": "<string>", "type": "chart|counter|timeline", "dataBinding": "<expression>" }`. Implementers will validate and document the full schema in README.
- Default Saved Games path is used only when `--dir` is not supplied and the OS is Windows; on other OSes, the user must specify `--dir` or the tool will attempt to discover the user profile path.
- The term `style` maps to an HTML/CSS template variant; template authors will supply both `elegant` and `colorful` assets.

## Out of Scope

- Live streaming of events or continuous monitoring (the tool is a run-once report generator). Persistent database storage and long-term retention are out of scope.

## Next Steps

1. Convert this spec into `specs/001-cmdrs-chronicle/plan.md` with implementation tasks (I can run `/speckit.plan` when you confirm).  
2. Confirm infographic JSON schema (if you have examples, attach them) — otherwise I will proceed with the assumed minimal schema above.  
