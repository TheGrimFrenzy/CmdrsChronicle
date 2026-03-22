# Research: CmdrsChronicle Implementation Stack and Best Practices

## Decision: C# 12 / .NET 8 (LTS), Self-contained Windows Console App

### Rationale
- .NET 8 (LTS) enables publishing a single-file, self-contained executable that runs on any supported Windows 10+ system with no need for pre-installed runtimes, interpreters, or VMs.
- C# 12 provides modern language features, strong typing, and robust ecosystem support for CLI tools, file I/O, JSON parsing, and HTML generation.
- The .NET SDK includes System.CommandLine for CLI parsing, System.Text.Json for efficient JSON handling, and System.IO for file operations—all without external dependencies.
- For complex infographic queries, an embedded in-memory database (LiteDB or SQLite in-memory mode via .NET) will be used. This allows for efficient ad-hoc querying and aggregation over parsed events, leveraging LINQ or SQL as appropriate.
- xUnit is the de facto standard for .NET unit/integration testing and integrates with all major CI systems.
- Library-first architecture aligns with project constitution and .NET best practices, enabling testability and future extensibility.

### Alternatives Considered
- **Python**: Easy for scripting, but requires user to have Python installed or to bundle an interpreter, increasing support burden and attack surface.
- **Rust**: Produces small, fast binaries, but has a steeper learning curve and less mature ecosystem for Windows desktop/CLI UX.
- **Go**: Good for static binaries, but less idiomatic for Windows-centric CLI tools and less integrated with .NET/Windows ecosystem.
- **C++**: Maximum control, but higher development and maintenance cost, and more error-prone for rapid CLI/report tooling.

### Best Practices
- Use .NET's self-contained publish mode to ensure zero external dependencies.
- Structure code as libraries with clear APIs and documentation.
- Use System.CommandLine for robust CLI argument parsing and help output.
- Use System.Text.Json for fast, schema-compliant JSON parsing.
- Use xUnit for all test automation; enforce test coverage in CI.
- Log all errors and warnings in a structured, user-visible format in the HTML report.
- Document all public APIs and provide a README for each library.
- Provide a standard Windows installer (MSIX or MSI) as part of the build pipeline to simplify distribution and installation for end users.

---

This research resolves all technology and stack clarifications for the implementation plan. No further research blockers remain.