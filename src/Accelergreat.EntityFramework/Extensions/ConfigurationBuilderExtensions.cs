using Accelergreat.Environments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Accelergreat.EntityFramework.Extensions;

public static class ConfigurationBuilderExtensions
{
    /// <summary>
    /// <para>Adds an database connection string to the <paramref name="configurationBuilder"/>.</para>
    /// </summary>
    public static IConfigurationBuilder AddEntityFrameworkDatabaseConnectionString<TDbContext>(
        this IConfigurationBuilder configurationBuilder, 
        string databaseConnectionStringName, 
        IReadOnlyAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData)
        where TDbContext : DbContext
    {
        var databaseConnectionString =
            accelergreatEnvironmentPipelineData.GetEntityFrameworkDatabaseConnectionString<TDbContext>();

        configurationBuilder.AddInMemoryCollection(new[]
        {
            new KeyValuePair<string, string?>($"ConnectionStrings:{databaseConnectionStringName}", databaseConnectionString)
        });

        return configurationBuilder;
    }
}