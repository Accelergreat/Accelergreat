using System.ComponentModel;
using Microsoft.Extensions.Configuration;

namespace Accelergreat.EntityFramework.SqlServer;


internal static class SqlServerEntityFrameworkConfigurationProvider
{
    internal static SqlServerEntityFrameworkConfiguration GetConfiguration(IConfiguration configuration)
    {
        var sqlHostConfiguration = new SqlServerEntityFrameworkConfiguration();

        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        configuration.GetSection(SqlServerEntityFrameworkConfiguration.ConfigurationSectionName)?.Bind(sqlHostConfiguration);

        return sqlHostConfiguration;
    }
}