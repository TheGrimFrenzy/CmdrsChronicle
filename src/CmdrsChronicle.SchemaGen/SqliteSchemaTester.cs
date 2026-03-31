using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace CmdrsChronicle.SchemaGen
{
    public static class SqliteSchemaTester
    {
        public static string TryCreateTablesSequentially(IEnumerable<string> sqlStatements)
        {
            using var conn = new SqliteConnection("Data Source=:memory:");
            conn.Open();
            int idx = 0;
            foreach (var sql in sqlStatements)
            {
                idx++;
                try
                {
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    return $"FAIL at statement #{idx}:\n{sql}\nError: {ex.Message}";
                }
            }
            return "OK";
        }
    }
}
