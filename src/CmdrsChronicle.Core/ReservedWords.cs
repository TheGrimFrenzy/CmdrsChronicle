using System.Collections.Generic;

namespace CmdrsChronicle.Core
{
    /// <summary>
    /// Maintains the list of reserved words for SQLite and provides prefixing logic for DB mapping.
    /// </summary>
    public static class ReservedWords
    {
        /// <summary>
        /// List of reserved words for SQLite (partial, extend as needed).
        /// </summary>
        // List includes all SQLite reserved words plus 'timestamp' and other SQL keywords per spec/plan.
        public static readonly HashSet<string> SQLite = new()
        {
            "abort", "action", "add", "after", "all", "alter", "analyze", "and", "as", "asc", "attach", "autoincrement", "before", "begin", "between", "by", "cascade", "case", "cast", "check", "collate", "column", "commit", "conflict", "constraint", "create", "cross", "current_date", "current_time", "current_timestamp", "database", "default", "deferrable", "deferred", "delete", "desc", "detach", "distinct", "drop", "each", "else", "end", "escape", "except", "exclusive", "exists", "explain", "fail", "for", "foreign", "from", "full", "glob", "group", "having", "if", "ignore", "immediate", "in", "index", "indexed", "initially", "inner", "insert", "instead", "intersect", "into", "is", "isnull", "join", "key", "left", "like", "limit", "match", "natural", "no", "not", "notnull", "null", "of", "offset", "on", "or", "order", "outer", "plan", "pragma", "primary", "query", "raise", "recursive", "references", "regexp", "reindex", "release", "rename", "replace", "restrict", "right", "rollback", "row", "savepoint", "select", "set", "table", "temp", "temporary", "then", "to", "transaction", "trigger", "union", "unique", "update", "using", "vacuum", "values", "view", "virtual", "when", "where", "with", "without",
            // Explicitly added for spec/plan compliance:
            "timestamp"
        };

        /// <summary>
        /// <summary>
        /// Returns the DB-safe column name, prefixing with 'event_' if the name is a reserved word.
        /// Per spec/plan, this must be applied to all event property names that are reserved SQL keywords (e.g., 'timestamp', 'order', 'group').
        /// </summary>
        public static string PrefixIfReserved(string name)
        {
            // Always check lower-case for reserved word match (SQLite is case-insensitive for keywords)
            return SQLite.Contains(name.ToLowerInvariant()) ? $"event_{name}" : name;
        }
    }
}
