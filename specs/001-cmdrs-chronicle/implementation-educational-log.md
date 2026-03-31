# Implementation Educational Log: CmdrsChronicle

This log documents the rationale, plan, and lessons learned for major implementation steps in the CmdrsChronicle project, supporting educational traceability and transparency.

---

## 1. Static SQL Schema Generator (with Parent-Child Relationships)

**Rationale:**
- Needed a robust, testable way to generate a static SQL schema from Elite Dangerous event schemas, ensuring all event types are represented as tables and relationships are explicit.
- Required to automate fetching schemas from GitHub, handle naming conventions, and support parent-child (foreign key) relationships for array-of-object properties.

**Plan:**
- Fetch all .json event schemas recursively from the GitHub repo, skipping Event.json.
- Use schema file names as table names.
- For each schema, generate a CREATE TABLE statement with synthetic PKs.
- For array-of-object properties, generate a child table with a foreign key to the parent.
- Output a single static SQL file for use in the runtime DB.

**Lessons Learned:**
- Automated schema fetching and naming conventions reduce manual errors.
- Parent-child logic requires careful property analysis and generator logic.
- GitHub API rate limits require token support.
- Test-first and modular design made refactoring and extension easier.

---

## 2. CLI Contract and Argument Parsing (T204)

**Rationale:**
- The CLI is the primary user interface for CmdrsChronicle; it must be robust, discoverable, and testable.
- Argument parsing should be explicit, user-friendly, and match the contract in contracts/cli.md and plan.md.

**Plan:**
- Replace the "Hello, World!" stub in CmdrsChronicle.Cli/Program.cs with a System.CommandLine-based CLI.
- Define root command and options as per the CLI contract.
- Parse arguments and print them as a stub (for now), to validate parsing logic.
- Prepare for future wiring to report generation logic and add tests for argument parsing and help output.

**Lessons Learned:**
- System.CommandLine provides a robust, modern CLI experience.
- Stubbing argument parsing before wiring up logic allows for incremental, testable development.
- Always document the plan and rationale before code changes to maintain educational traceability.

---

## Next Steps
- Extend the CLI to wire up report generation logic.
- Add tests for argument parsing and help output.
- Maintain this log for all major implementation steps.
