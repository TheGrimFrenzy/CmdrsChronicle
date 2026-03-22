- Write tests for file discovery, parallel parsing, and concurrency cap logic (test-first, before implementation)
  - Ensure CI enforces green tests for all parallel/concurrent code paths
  - Write tests for parallel infographic rendering and concurrency cap (test-first, before implementation)
  - Ensure CI enforces green tests for all parallel/concurrent code paths
> 
> **Plan Path:** specs/001-cmdrs-chronicle/plan.md
> **Data Model:** specs/001-cmdrs-chronicle/data-model.md



# Requirement-to-Task Mapping
| Requirement/User Story         | Task(s)         |
|-------------------------------|-----------------|
| Tagline selection             | T101, T301      |
| Infographics directory        | T102            |
| No data message logic         | T305            |
| Parallel file parsing         | T302, T304      |
| SQLite event storage          | T203, T303      |
| CLI contract                  | T204, T306      |
| Installer packaging           | T601, T602      |
| End-user instructions         | T602            |
| Performance/memory NFRs       | T605            |

# Glossary
- **Infographic tile**: A single visual element in the report, rendered from a JSON definition.
- **Infographic category**: A grouping of related infographic tiles (e.g., Travel, Combat).
- **No data message**: A contextually relevant message shown when no valid events exist for the report timeframe.
- **Tagline**: A short, random phrase displayed in the report masthead.
- **MVP**: Minimum Viable Product; the smallest set of features that delivers user value.
- **NFR**: Non-Functional Requirement (e.g., performance, memory usage).

// Clarifying reference to the data model can be found in data-model.md

> **Feature:** CmdrsChronicle (Self-contained HTML report from Elite Dangerous logs)
> 
> **Spec Path:** specs/001-cmdrs-chronicle/spec.md
> **Plan Path:** specs/001-cmdrs-chronicle/plan.md
> **Data Model:** specs/001-cmdrs-chronicle/data-model.md

---

## 1. Phase 1: Setup
1. [ ] T101 Create tagline file for report masthead
   - **Acceptance Criteria:**
     - templates/taglines.txt exists with at least 5 non-comment lines
     - README documents the file and format
  - Steps:
    - Add templates/taglines.txt with one tagline per line; lines starting with # are comments and ignored
    - Document the file and its format in README
2. [ ] T102 Generate infographics directory structure
   - **Acceptance Criteria:**
     - /infographics directory exists with subdirectories for all categories in spec.md
     - README documents the structure
  - Steps:
    - Create an /infographics directory at the project root
    - For each identified infographic category (see spec.md), create a subdirectory under /infographics (e.g., /infographics/Travel, /infographics/Combat, etc.)
    - Document the directory structure in README for onboarding

3. [ ] T103 Create initial project structure for library-first .NET solution
   - **Acceptance Criteria:**
     - Solution and all main project folders created as per plan.md
     - .gitignore, README.md, .sln present
     - Standard test directory layout present
  - Steps:
    - Create solution and main project folders: src/CmdrsChronicle.Core, src/CmdrsChronicle.Cli, src/CmdrsChronicle.Infographics, src/CmdrsChronicle.Tests, src/Installer
    - Add .gitignore, README.md, and initial .sln file
    - Set up standard directory layout for tests/unit, tests/integration, tests/contract
    - Document the folder structure in README for clarity

4. [ ] T104 Add .NET 8 SDK and configure solution for C# 12
   - **Acceptance Criteria:**
     - All csproj files target net8.0 and C# 12
     - global.json pins SDK if needed
     - README documents requirements
  - Steps:
    - Set target framework to net8.0 in all csproj files
    - Configure language version to 12.0 in Directory.Build.props or each csproj
    - Add global.json if needed to pin SDK
    - Document SDK and language version requirements in README

5. [ ] T105 Add xUnit test project and configure CI for build/test
   - **Acceptance Criteria:**
     - xUnit project added, sample test runs
     - CI pipeline runs build and test
     - CONTRIBUTING.md documents test execution
  - Steps:
    - Add xUnit to src/CmdrsChronicle.Tests
    - Add a sample test class for smoke testing
    - Set up GitHub Actions or Azure Pipelines for build and test
    - Document test execution in CONTRIBUTING.md

6. [ ] T106 Add documentation stubs (README, CONTRIBUTING, usage)
   - **Acceptance Criteria:**
     - README.md, CONTRIBUTING.md, USAGE.md exist as stubs in correct locations
     - All link to quickstart.md and spec.md
  - Steps:
    - Create README.md in repo root and each main project folder
    - Add CONTRIBUTING.md and USAGE.md as stubs
    - Link to quickstart.md and spec.md for onboarding


## 2. Phase 2: Foundational

1. [ ] T201 Implement core data model classes in CmdrsChronicle.Core
   - **Acceptance Criteria:**
     - All models defined as per data-model.md
     - Unit tests verify property mapping
     - XML doc comments present
  - Steps:
    - Write unit tests for each model to verify property mapping (test-first)
    - Define LogFile, LogEvent, InfographicDefinition, Report, ReportSection as per data-model.md
    - Add XML doc comments for all properties to aid learning
    - Ensure reserved word handling for DB mapping (prefix with event_ for SQLite reserved words)

2. [ ] T202 Implement Elite Dangerous journal schema loader and reserved word logic
   - **Acceptance Criteria:**
     - Reserved word handling logic matches plan.md and spec.md
     - Tests verify prefixing for SQLite reserved words
     - Reserved word list documented in code
  - Steps:
    - Write tests to verify reserved word handling (test-first)
    - Add code to load event schemas from canonical repo (see plan.md)
    - Implement reserved word prefixing for SQL mapping (maintain reserved word list for SQLite)
    - Document reserved word list and logic in code comments

3. [ ] T203 Implement in-memory event storage and relational mapping using SQLite
   - **Acceptance Criteria:**
     - Integration tests verify schema mapping and data insertion
     - SQLite in-memory DB used for each run
     - All event schemas mapped to tables as per data-model.md
  - Steps:
    - Write integration tests to verify schema mapping and data insertion (test-first)
    - Integrate Microsoft.Data.Sqlite or System.Data.SQLite NuGet package
    - Initialize SQLite in-memory database for each report generation run
    - For each event schema, create a table in SQLite (table-per-schema)
    - Map event properties to columns, handling reserved words and complex types (child tables for objects/arrays, JSON truncation for deep nesting)
    - Add synthetic primary key event_id to each table
    - Document why SQLite in-memory is used (ad-hoc queries, no persistence, cross-platform)

4. [ ] T204 Implement CLI contract and argument parsing in CmdrsChronicle.Cli
   - **Acceptance Criteria:**
     - Argument parsing supports all CLI options from contracts/cli.md
     - --help output is detailed
     - Tests cover argument parsing and help
  - Steps:
    - Write tests for argument parsing and help output (test-first)
    - Use System.CommandLine for robust argument parsing
    - Support all CLI options from contracts/cli.md
    - Add --help output with detailed descriptions


## 3. Phase 3: [US1] Generate a weekly summary report (P1)
1. [ ] T301 Implement report tagline selection logic
   - **Acceptance Criteria:**
     - Random non-comment line from templates/taglines.txt is used as masthead tagline
     - Test ensures only non-comment lines are selected and taglines rotate randomly
  - Steps:
    - On report generation, select a random non-comment line from templates/taglines.txt as the masthead tagline
    - Write a test to ensure only non-comment lines are selected and that taglines rotate randomly

2. [ ] T302 [P] [US1] Implement journal file discovery and parsing
   - **Acceptance Criteria:**
     - File discovery, parallel parsing, and concurrency cap logic tested
     - Directory scan and filename validation as per research-naming.md
     - Parallelism uses TPL with concurrency cap
  - Steps:
    - Write tests for file discovery, parallel parsing, and concurrency cap logic (test-first)
    - Scan default or user-specified directory for journal files (see research-naming.md for pattern)
    - Validate filename pattern and file accessibility
    - Read and parse journal files in parallel using Task Parallel Library (TPL) with a concurrency cap (set MaxDegreeOfParallelism to Environment.ProcessorCount or configurable value)
    - Use Parallel.ForEach or Task.Run for parallelism; avoid creating raw threads
    - Parse each line as JSON, skip invalid lines, log errors for diagnostics
    - Document in code and README why parallelism and concurrency cap are used (performance, resource safety)

3. [ ] T303 [P] [US1] Store parsed events in SQLite in-memory DB and aggregate for report
   - **Acceptance Criteria:**
     - Valid events inserted into SQLite in-memory DB
     - Aggregation queries produce correct summary/timeline/infographics
     - Tests for event insertion and aggregation
  - Steps:
    - Insert valid events into SQLite in-memory DB using schema mapping logic
    - Use SQL queries to aggregate events for summary, timeline, and infographics
    - Handle empty or malformed files gracefully (log errors, skip bad lines)
    - Write tests for event insertion and aggregation queries

4. [ ] T304 [P] [US1] Implement HTML report generation (summary, timeline, infographics)
   - **Acceptance Criteria:**
     - HTML template for summary report created (elegant style default)
     - Infographic tiles rendered from /infographics JSON files by category
     - Parallel rendering logic tested
  - Steps:
    - Create HTML template for summary report (elegant style default)
    - Embed CSS, JS, and assets inline for self-contained output
    - Render infographic tiles from /infographics JSON files by category using data from SQLite DB
    - Render infographic tiles from /infographics JSON files in parallel (one task per tile) using TPL with concurrency cap (MaxDegreeOfParallelism)
    - Use Parallel.ForEach or Task.Run for parallel infographic tile rendering; document concurrency cap logic
    // Terminology: 'infographic tile' = single visual element; 'infographic category' = grouping; 'infographics' = overall feature
    - Add diagnostics/logging section to report (parsing errors, files scanned, event counts)
    - Write tests for parallel infographic rendering and concurrency cap
    - Document in code and README why parallelism and concurrency cap are used (performance, resource safety)

5. [ ] T305 [US1] Implement 'no data' report logic and message selection
   - **Acceptance Criteria:**
     - No valid events triggers message selection by ordinal day
     - Leap year logic correct
     - Placeholders interpolated
     - Fallback/default message logic works
  - Steps:
    - Detect when no valid events exist in timeframe (query SQLite DB for event count)
    - Load no-data message from templates/no-data-messages.json using the current ordinal day of the year (1-365)
    - For leap years, subtract 1 from the ordinal day after Feb 29 to re-align with the holiday/message entries (i.e., Mar 1 is day 60, Mar 2 is day 61, etc.)
    - Use string interpolation to insert {cmdrName}, {lastSystem}, and {lastDate} into the selected message
    - If no message exists for the day, fallback to a default or random message
    - Extract Cmdr name and last system from most recent prior journal if available (query SQLite DB for latest event before start date)
    - Populate report with fallback details if none found (mark as unknown)
    - Write tests for 'no data' scenarios, including day-based selection and fallback

6. [ ] T306 [US1] Add CLI exit code and error handling logic
   - **Acceptance Criteria:**
     - Exit code 0 for success, non-zero for unrecoverable errors
     - Parsing errors logged in report, not console
     - Tests for exit code and error handling
  - Steps:
    - Return 0 for successful report (including 'no data')
    - Return non-zero only for unrecoverable errors
    - Log parsing errors in report, not to console
    - Write tests for exit code and error handling


## 4. Phase 4: [US2] Generate a report filtered by system (P2)

1. [ ] T401 [P] [US2] Implement --type by-system grouping and filtering
   - **Acceptance Criteria:**
     - CLI option for --type by-system present
     - Events grouped by star system in report
     - Tests for system grouping/filtering
  - Steps:
    - Add CLI option for --type by-system
    - Group events by star system ("by-system") using SQL queries in SQLite DB
    - Render per-system sections in report, showing grouped events and infographics
    - Write tests for system grouping and filtering

2. [ ] T402 [US2] Support combining --type by-system with --category and date filters
   - **Acceptance Criteria:**
     - --category and --start/--end filters work with by-system
     - Combined filters reflected in all report sections
     - Tests for combined filtering
  - Steps:
    - Allow --category and --start/--end to further filter/group events (apply as SQL WHERE clauses)
    - Ensure report reflects combined filters in all sections
    - Write tests for combined filtering logic


## 5. Phase 5: [US3] Custom date range, style, and category filtering (P3)

1. [ ] T501 [P] [US3] Implement --start, --end, --style, and --category options
   - **Acceptance Criteria:**
     - CLI options for all filters present
     - Arguments validated and parsed
     - Filters applied to DB queries and report
     - Tests for argument validation and filter application
  - Steps:
    - Add CLI options for all filters (date range, style, category)
    - Validate and parse date/style/category arguments
    - Apply filters to SQLite DB queries and report generation
    - Write tests for argument validation and filter application

2. [ ] T502 [US3] Implement style switching and category ordering in report
   - **Acceptance Criteria:**
     - Style switching logic loads correct template
     - Category ordering matches --category argument
     - Tests for style switching and ordering
  - Steps:
    - Load and apply correct HTML/CSS template for selected style (elegant or colorful)
    - Order categories in report as per --category argument (apply ordering in SQL or post-query)
    - Document style and category logic in code comments
    - Write tests for style switching and category ordering


## 6. Final Phase: Polish & Cross-Cutting
1. [ ] T601 Add installer project and build pipeline for MSIX/MSI
   - **Acceptance Criteria:**
     - Installer project created
     - Build produces MSIX or MSI
     - Installer instructions in README
  - Steps:
    - Write clear, step-by-step instructions for end users explaining how to install, configure, and use the application
    - Include details on customizing infographics and taglines
    - Note: The 'no data' messages are embedded and intentionally left as a surprise/mystery for the user
    - Add usage examples and troubleshooting tips
    - Place instructions in USAGE.md and link from README

2. [ ] T602 Generate end-user instructions and usage guide
   - **Acceptance Criteria:**
     - USAGE.md and README include clear end-user instructions
     - Infographics directory packaged and customizable
     - Tests for installer build
  - Steps:
    - Create Installer/ project for packaging
    - Package the /infographics directory and all category subdirectories so they are delivered with the app
    - Ensure the application reads infographics from the installed directory, allowing end users to add or modify their own
    - Configure build to produce MSIX or MSI
    - Add installer instructions to README
    - Write tests for installer build

3. [ ] T603 Add end-to-end test scenarios and documentation
   - **Acceptance Criteria:**
     - Integration tests for all user stories
     - Usage examples in USAGE.md and README
     - Troubleshooting/diagnostics documented
  - Steps:
    - Write integration tests for all user stories (journal parsing, DB mapping, report generation)
    - Add usage examples to USAGE.md and README
    - Document troubleshooting and diagnostics in README

4. [ ] T604 Review, refactor, and document all code for maintainability
   - **Acceptance Criteria:**
     - Code reviewed for clarity and learning value
     - Comments/explanations for non-expert C# developers
     - All public APIs/classes have XML docs

5. [ ] T605 Add performance and memory usage tests for non-functional requirements
   - **Acceptance Criteria:**
     - Performance test: report generation <30s for 1 week of logs
     - Memory test: <500MB RAM for 10,000 events
     - Test methodology/results documented in README/CONTRIBUTING.md
     - Tests run in CI and fail if constraints not met
   - **NFR Reference:** See spec.md and plan.md for explicit performance/memory goals
  - Steps:
    - Write a performance test to ensure report generation completes in <30s for 1 week of logs (simulate or use sample data)
    - Write a memory usage test to verify the application stays under 500MB RAM for 10,000 events
    - Document test methodology and results in README or CONTRIBUTING.md
    - Ensure these tests run in CI and fail if constraints are not met
  - Steps:
    - Review all code for clarity and learning value
    - Add comments and explanations for non-expert C# developers
    - Ensure all public APIs and classes have XML docs
    - Write a final pass of code review checklist


---

## 7. Dependencies

- US1 → US2 → US3 (stories are independently testable, but US2/US3 build on US1 foundation)
- Foundational tasks must be completed before any user story phase

---

## 8. Parallel Execution Examples

 - T302, T303, T304 can be developed in parallel (journal parsing, DB storage, report generation)
 - T401, T402 can be developed in parallel (system grouping, filter logic)
 - T501, T502 can be developed in parallel (date/style/category filtering, style switching)


## 9. Implementation Strategy


The implementation proceeds in clearly defined, test-first phases:

- **MVP:** Complete all tasks for US1 (Phase 3) to deliver a weekly summary report with default options.
- **Incremental Delivery:** Add system grouping (US2), then advanced filtering and style (US3), each as an independently testable phase.
- **Parallelization:** Where possible, develop journal parsing, DB storage, and report generation in parallel (see Parallel Execution Examples).
- **Cross-Cutting:** Polish, document, and validate non-functional requirements in the final phase.

Each phase is independently testable and delivers user value. All code is developed test-first, with CI enforcing green tests for all major features and concurrency logic.

*Generated by speckit.tasks agent — March 21, 2026*
