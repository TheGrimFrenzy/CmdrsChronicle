using System.Collections.Generic;

namespace CmdrsChronicle.Core
{
    /// <summary>
    /// A logical section of a report, optionally associated with a star system visit.
    /// When <see cref="SystemName"/> is null the section renders without a divider header,
    /// which is the default behaviour for the standard (non-by-system) report.
    /// </summary>
    /// <param name="SystemName">Star system name used as a divider header, or <see langword="null"/> for a system-less section.</param>
    /// <param name="ArrivalLore">Arrival date formatted in Elite lore years (real year + 1286), e.g. <c>"3312-04-01"</c>.</param>
    /// <param name="ArrivalActual">Arrival date as an actual calendar date string, e.g. <c>"2026-04-01"</c>.</param>
    /// <param name="Results">Infographic query results belonging to this section.</param>
    public sealed record SystemVisit(
        string? SystemName,
        string? ArrivalLore,
        string? ArrivalActual,
        IReadOnlyList<InfographicQueryResult> Results);
}
