# CmdrsChronicleDotNet Constitution

<!--
	Sync Impact Report
	- Version change: template → 1.0.0
	- Modified principles: placeholders → concrete principles (Library-First; CLI Interface; Test-First; Integration Testing; Observability & Versioning & Simplicity)
	- Added sections: Additional Constraints (technology stack) and Development Workflow
	- Removed sections: none
	- Templates requiring updates: .specify/templates/plan-template.md (✅ updated), .specify/templates/spec-template.md (⚠ pending), .specify/templates/tasks-template.md (⚠ pending)
	- Deferred TODOs: none
-->

## Core Principles

### VI. Single-Developer, Non-Expert Friendly
This project is being built by a single developer who is not an expert in C# or .NET. All code implementations and iterative changes MUST be accompanied by clear explanations of what is being done and why, to maximize maintainability and learning. Documentation and code comments should be prioritized to support onboarding and future self-review.

### I. Library-First
Every feature or service MUST be conceived as a small, well-defined library or package
that encapsulates its behavior, API, and tests. Libraries MUST be independently
buildable and testable; they MUST include a clear README describing purpose,
public API, and example usage. Organizational-only libraries (no runtime purpose)
are PROHIBITED.

### II. CLI Interface
Where applicable, libraries and tools SHOULD expose a command-line interface
or thin CLI adapter that supports both human-readable and machine-readable
outputs (JSON). Text I/O (stdin/stdout/stderr) SHOULD be supported to simplify
automation, debugging, and CI integration.

### III. Test-First (NON-NEGOTIABLE)
Testing discipline is mandatory: unit tests and contract tests MUST be written
before implementation work. Tests MUST fail initially, then pass after the
implementation (Red-Green-Refactor). All code merged to the main branch MUST
have green tests in CI.

### IV. Integration Testing
Integration tests MUST cover cross-library contracts, public interfaces, and any
external integrations. Critical integration tests (data contracts, auth flows,
schema migrations) MUST run in CI and be part of the release gating.

### V. Observability, Versioning & Simplicity
- Observability: Libraries and services MUST emit structured logs and expose
	actionable telemetry where applicable. Errors MUST be logged with context.
- Versioning: The project follows semantic versioning for public artifacts
	(MAJOR.MINOR.PATCH). Breaking changes require a MAJOR bump and a migration
	guide. API compatibility expectations MUST be documented for public packages.
- Simplicity: Prefer simple, well-documented solutions. Complex patterns require
	explicit justification in the plan and a documented rollback strategy.

## Additional Constraints

Technology stack: This repository targets .NET (C#) runtimes and follows the
conventions of .NET projects (csproj, nuget package metadata). CI pipelines
MUST restore, build, test, and lint for supported SDK versions declared in
project files.

Security & compliance: Secrets MUST NOT be committed. Security-sensitive changes
MUST include threat reasoning and, where applicable, a short threat model.

## Development Workflow

- Pull requests are the primary mechanism for changes. Every PR MUST include
	a descriptive summary, a linked issue or spec, and at least two approvals from
	maintainers for non-trivial changes.
- CI gates: restore/build/test/lint MUST pass before merge. For changes that
	affect public APIs or packages, a compatibility check and release notes are
	required.
- Releases: Build artifacts for releases MUST include changelogs and any
	migration notes required by the Constitution.

## Governance

Amendments: Changes to this constitution MUST be proposed as a repository PR
targeting `.specify/memory/constitution.md`. Amendments that add or materially
change principles or governance language require a MINOR version bump; removals
or redefinitions of principles require a MAJOR bump. All amendment PRs MUST
include rationale and a migration plan when the change affects existing work.

Approval: For non-trivial amendments, approval requires at least two maintainer
approvals or a majority of the project's core-maintainers team as defined in
repository settings. Trivial editorial changes (typos, clarifications) MAY be
merged after one maintainer approval and a passing CI.

Versioning policy: The Constitution uses semantic versioning for governance
changes:
- MAJOR: Backward-incompatible governance or principle removals/redefinitions.
- MINOR: New principle or materially expanded guidance.
- PATCH: Clarifications, wording, typo fixes, and non-semantic refinements.

Compliance review: Major and Minor amendments MUST include a compliance review
task in the PR checklist to update any templates, plans, or task guidelines that
reference constitution-driven gates.

**Version**: 1.0.0 | **Ratified**: 2026-03-21 | **Last Amended**: 2026-03-21
