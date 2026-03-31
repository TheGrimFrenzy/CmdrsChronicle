using System;
using System.Collections.Generic;

namespace CmdrsChronicle.Core
{
    /// <summary>
    /// Holds the results of running an infographic tile's queries against the SQLite DB.
    /// Produced by <see cref="InfographicQueryRunner"/>; consumed sequentially by a renderer.
    /// </summary>
    public class InfographicQueryResult
    {
        /// <summary>The definition this result was produced from.</summary>
        public InfographicDefinition Definition { get; init; } = null!;

        /// <summary>Main metric value (scalar result of <see cref="InfographicDefinition.Query"/>).
        /// If the main query returns multiple columns, this is the first column's numeric value.</summary>
        public long MainValue { get; init; }

        /// <summary>
        /// Additional scalar values returned by the main query, keyed by column name (case-insensitive).
        /// This allows `Query` to return multiple columns (e.g. `COUNT(*) AS count, SUM(Reward) AS totalReward`).
        /// </summary>
        public IReadOnlyDictionary<string, long> Scalars { get; init; } = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Detail rows for chart rendering — (label, count) pairs returned by
        /// <see cref="InfographicDefinition.DetailQuery"/>, in query-result order.
        /// </summary>
        public IReadOnlyList<(string Label, long Value)> DetailRows { get; init; }
            = Array.Empty<(string, long)>();

        /// <summary>True when <see cref="MainValue"/> meets or exceeds <see cref="InfographicDefinition.Threshold"/>.</summary>
        public bool MeetsThreshold => MainValue >= Definition.Threshold;

        /// <summary>Duration spent executing the main query.</summary>
        public TimeSpan MainQueryDuration { get; init; }

        /// <summary>Duration spent executing the detail query (if any).</summary>
        public TimeSpan DetailQueryDuration { get; init; }

        /// <summary>Total duration spent querying for this tile (main + detail).</summary>
        public TimeSpan TotalQueryDuration => MainQueryDuration + DetailQueryDuration;
    }
}
