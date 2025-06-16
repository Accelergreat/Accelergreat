using Accelergreat.EntityFramework.Extensions;
using Accelergreat.EntityFramework.SqlServer.Transactions.Overrides;
using Accelergreat.Environments;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Accelergreat.EntityFramework.SqlServer.Transactions.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a <typeparamref name="TDbContext"/> configured to use SQL Server with same the connection and transaction that is used with the test database. This should only be used when the reset strategy is set to Transactions.
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <param name="services"></param>
    /// <param name="accelergreatEnvironmentPipelineData">See <see cref="AccelergreatEnvironmentPipelineDataKeys"/> for pre-defined keys.</param>
    /// <param name="sqlServerOptionsAction"></param>
    /// <param name="useTransactionOverriding">If set to true, Accelergreat will intercept <see cref="DatabaseFacade.BeginTransaction()"/> <see cref="DatabaseFacade.BeginTransactionAsync(CancellationToken)"/> by returning the transaction that is used with the test database. Commits and Rollbacks will be translated to use savepoints. By default, <paramref name="useTransactionOverriding"/> is set to false.</param>
    /// <returns></returns>
    public static IServiceCollection AddSqlServerDbContextUsingTransaction<TDbContext>(
        this IServiceCollection services,
        IReadOnlyAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData,
        Action<SqlServerDbContextOptionsBuilder>? sqlServerOptionsAction = null,
        bool useTransactionOverriding = false)
        where TDbContext : DbContext
    {
        var connection = (SqlConnection)
            accelergreatEnvironmentPipelineData.GetEntityFrameworkDatabaseConnection<TDbContext>();

        var transactionWallet = 
            accelergreatEnvironmentPipelineData.GetEntityFrameworkDatabaseTransactionWallet<TDbContext, SqlTransaction>();

        var transactionSqlServerDbContextFactory = new TransactionSqlServerDbContextFactory<TDbContext>(
            connection,
            transactionWallet,
            sqlServerOptionsAction);

        services.RemoveAll<DbContextOptions<TDbContext>>();
        services.RemoveAll<DbContextOptions>();
        services.RemoveAll<Microsoft.EntityFrameworkCore.IDbContextFactory<TDbContext>>();
        services.RemoveAll<TDbContext>();
        services.RemoveAll<DbContext>();

        services.AddSingleton(transactionWallet);

        services.AddDbContextFactory<TDbContext, AccelergreatSqlServerTransactionDbContextFactoryOverride<TDbContext>>(
            options =>
            {
                var typedOptions = (DbContextOptionsBuilder<TDbContext>) options;

                transactionSqlServerDbContextFactory.ConfigureOptions(typedOptions);

                if (useTransactionOverriding)
                {
                    transactionSqlServerDbContextFactory.SetupTransactionOverriding(typedOptions);
                }
            });

        services.AddScoped(serviceProvider => serviceProvider
            .GetRequiredService<Microsoft.EntityFrameworkCore.IDbContextFactory<TDbContext>>()
            .CreateDbContext());

        if (typeof(TDbContext) != typeof(DbContext))
        {
            services.AddScoped<DbContext>(serviceProvider => serviceProvider
                .GetRequiredService<Microsoft.EntityFrameworkCore.IDbContextFactory<TDbContext>>()
                .CreateDbContext());
        }

        return services;
    }
}