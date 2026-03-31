# Contributing to CmdrsChronicle

Thank you for your interest in contributing!

## Running Tests

- Ensure you have the .NET 8 SDK installed (see README).
- To run all tests:
  ```
  dotnet test
  ```
- Tests are located in `src/CmdrsChronicle.Tests` and organized by feature area.
- A sample test (`SmokeTests.cs`) verifies the test infrastructure is working.

## Continuous Integration (CI)

- All pull requests and pushes to main/feature branches trigger CI via GitHub Actions.
- CI runs `dotnet build` and `dotnet test` to ensure code quality.

## Further Reading
- See `README.md` for project overview and requirements.
- See `specs/001-cmdrs-chronicle/spec.md` for detailed specification.
