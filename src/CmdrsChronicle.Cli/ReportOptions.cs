namespace CmdrsChronicle.Cli;

/// <summary>
/// All user-facing options that drive report generation.
/// Shared between the CLI argument handler and the interactive setup UI.
/// </summary>
internal sealed record ReportOptions(
    string? Input,
    string? Output,
    string? Start,
    string? End,
    string? Type,
    string? Category,
    string? Style,
    string? Sort,
    int?    MaxParallelism
);
