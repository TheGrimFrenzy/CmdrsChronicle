# CmdrsChronicle.Tests

Test project for CmdrsChronicle.

## Performance and Memory Usage Tests (T605)

This project includes automated tests for non-functional requirements:

- **Performance:**
	- `ReportGeneration_CompletesUnder30Seconds_ForOneWeek` simulates 7 days of journal logs (100 events/day) and asserts report generation completes in under 30 seconds.
- **Memory Usage:**
	- `ReportGeneration_MemoryUsage_Under500MB_For10000Events` simulates 10,000 events and asserts peak memory usage stays under 500MB.

**Methodology:**
- Tests generate temporary journal files and invoke the CLI via `dotnet run`.
- Peak memory is measured using the child process's `PeakWorkingSet64`.
- Both tests are marked with a 35s timeout for CI enforcement.

**Results:**
- Tests must pass in CI for all changes. Failures indicate a regression in performance or memory usage.

- See [../..//quickstart.md](../../quickstart.md) for onboarding.
- See [../../specs/001-cmdrs-chronicle/spec.md](../../specs/001-cmdrs-chronicle/spec.md) for the full specification.
