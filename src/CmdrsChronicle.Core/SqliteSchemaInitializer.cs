using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using System.IO;

namespace CmdrsChronicle.Core
{
    /// <summary>
    /// Utility for initializing and verifying the in-memory SQLite database from the static SQL schema file.
    /// </summary>
    public static class SqliteSchemaInitializer
    {
        /// <summary>
        /// Loads the static SQL schema file and executes it against a new in-memory SQLite database.
        /// </summary>
        /// <param name="schemaFilePath">Path to the static SQL schema file.</param>
        /// <returns>SqliteConnection with schema loaded (caller is responsible for disposing).</returns>
        public static SqliteConnection CreateInMemoryDbWithSchema(string schemaFilePath)
        {
            if (!File.Exists(schemaFilePath))
                throw new FileNotFoundException($"Schema file not found: {schemaFilePath}");

            var sql = File.ReadAllText(schemaFilePath);
            var conn = new SqliteConnection("Data Source=:memory:");
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
            return conn;
        }

        /// <summary>
        /// Creates a named shared in-memory SQLite database and applies the schema.
        /// Multiple connections opened with the same <paramref name="dbName"/> share the same
        /// in-memory data, enabling a parallel-read connection pool. The returned connection
        /// must remain open for the lifetime of the shared database.
        /// </summary>
        /// <param name="schemaFilePath">Path to the static SQL schema file.</param>
        /// <param name="dbName">Logical name for the shared in-memory database.</param>
        /// <returns>Open keeper connection (caller must dispose to release the database).</returns>
        public static SqliteConnection CreateSharedInMemoryDb(string schemaFilePath, string dbName = "reportdb")
        {
            if (!File.Exists(schemaFilePath))
                throw new FileNotFoundException($"Schema file not found: {schemaFilePath}");

            var sql  = File.ReadAllText(schemaFilePath);
            var conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
            return conn;
        }

        /// <summary>
        /// Returns a list of all table names in the given SQLite connection.
        /// </summary>
        public static List<string> GetTableNames(SqliteConnection conn)
        {
            var tables = new List<string>();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' ORDER BY name;";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                tables.Add(reader.GetString(0));
            }
            return tables;
        }
    }
}
