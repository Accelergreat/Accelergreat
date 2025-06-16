using System.ComponentModel;
using Microsoft.Data.Sqlite;

namespace Accelergreat.EntityFramework.Sqlite;


internal static class SqliteConnectionStringProvider
{
    internal static string GetConnectionString(string databaseName)
    {
        var sqliteConnectionStringBuilder = new SqliteConnectionStringBuilder
        {
            DataSource = databaseName,
            Mode = SqliteOpenMode.Memory,
            Cache = SqliteCacheMode.Shared
        };

        return sqliteConnectionStringBuilder.ConnectionString;
    }
}