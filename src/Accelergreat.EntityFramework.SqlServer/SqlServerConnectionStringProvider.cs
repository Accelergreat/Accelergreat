using System.ComponentModel;
using Microsoft.Data.SqlClient;

namespace Accelergreat.EntityFramework.SqlServer;


internal static class SqlServerConnectionStringProvider
{
    internal static string GetConnectionString(string databaseName, SqlServerEntityFrameworkConfiguration configuration)
    {
        var sqlConnectionStringBuilder = !string.IsNullOrWhiteSpace(configuration.ConnectionString)
            ? new SqlConnectionStringBuilder(configuration.ConnectionString)
            : new SqlConnectionStringBuilder();

        sqlConnectionStringBuilder.InitialCatalog = databaseName;

        if (!sqlConnectionStringBuilder.IntegratedSecurity && string.IsNullOrWhiteSpace(sqlConnectionStringBuilder.UserID) || string.IsNullOrWhiteSpace(sqlConnectionStringBuilder.Password))
        {
            sqlConnectionStringBuilder.IntegratedSecurity = true;
        }

        if (string.IsNullOrWhiteSpace(sqlConnectionStringBuilder.DataSource))
        {
            sqlConnectionStringBuilder.DataSource = "(localdb)\\MSSQLLocalDB";
        }

        return sqlConnectionStringBuilder.ConnectionString;
    }
}