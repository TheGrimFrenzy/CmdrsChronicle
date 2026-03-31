using System.Collections.Generic;

namespace CmdrsChronicle.Core
{
    /// <summary>
    /// A logical section of a report, optionally associated with a star system visit.
    /// When <see cref="SystemName"/> is null the section renders without a divider header,
    /// which is the default behaviour for the standard (non-by-system) report.
    /// </summary>
    public sealed record SystemVisit(
        string? SystemName,
        string? ArrivalLore,
        string? ArrivalActual,
        IReadOnlyList<InfographicQueryResult> Results);
}
