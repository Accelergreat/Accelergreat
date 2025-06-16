using System.Data.Common;
using Accelergreat.Environments;
using Microsoft.EntityFrameworkCore;

namespace Accelergreat.EntityFramework.Extensions;

public static class AccelergreatEnvironmentPipelineDataEntityFrameworkExtensions
{
    public static string GetEntityFrameworkDatabaseConnectionString<TDbContext>(
        this IReadOnlyAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData)
        where TDbContext : DbContext
    {
        return accelergreatEnvironmentPipelineData.Get<string>(
            AccelergreatEnvironmentPipelineDataKeys.EntityFrameworkDatabaseConnectionString<TDbContext>());
    }

    internal static void AddEntityFrameworkDatabaseConnectionString<TDbContext>(
        this IAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData,
        string connectionString)
        where TDbContext : DbContext
    {
        accelergreatEnvironmentPipelineData.Add(
            AccelergreatEnvironmentPipelineDataKeys.EntityFrameworkDatabaseConnectionString<TDbContext>(),
            connectionString);
    }

    public static string GetEntityFrameworkDatabaseName<TDbContext>(
        this IReadOnlyAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData)
        where TDbContext : DbContext
    {
        return accelergreatEnvironmentPipelineData.Get<string>(
            AccelergreatEnvironmentPipelineDataKeys.EntityFrameworkDatabaseName<TDbContext>());
    }

    internal static void AddEntityFrameworkDatabaseName<TDbContext>(
        this IAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData,
        string databaseName)
        where TDbContext : DbContext
    {
        accelergreatEnvironmentPipelineData.Add(
            AccelergreatEnvironmentPipelineDataKeys.EntityFrameworkDatabaseName<TDbContext>(),
            databaseName);
    }

    public static DbConnection GetEntityFrameworkDatabaseConnection<TDbContext>(
        this IReadOnlyAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData)
        where TDbContext : DbContext
    {
        return accelergreatEnvironmentPipelineData.Get<DbConnection>(
            AccelergreatEnvironmentPipelineDataKeys.EntityFrameworkDatabaseConnection<TDbContext>());
    }

    internal static void AddEntityFrameworkDatabaseConnection<TDbContext>(
        this IAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData,
        DbConnection connection)
        where TDbContext : DbContext
    {
        accelergreatEnvironmentPipelineData.Add(
            AccelergreatEnvironmentPipelineDataKeys.EntityFrameworkDatabaseConnection<TDbContext>(),
            connection);
    }

    public static IDbTransactionWallet GetEntityFrameworkDatabaseTransactionWallet<TDbContext>(
        this IReadOnlyAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData)
        where TDbContext : DbContext
    {
        return accelergreatEnvironmentPipelineData.Get<IDbTransactionWallet>(
            AccelergreatEnvironmentPipelineDataKeys.EntityFrameworkDatabaseTransactionWallet<TDbContext>());
    }

    public static IDbTransactionWallet<TDbTransaction> GetEntityFrameworkDatabaseTransactionWallet<TDbContext, TDbTransaction>(
        this IReadOnlyAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData)
        where TDbContext : DbContext
        where TDbTransaction : DbTransaction
    {
        return accelergreatEnvironmentPipelineData.Get<IDbTransactionWallet<TDbTransaction>>(
            AccelergreatEnvironmentPipelineDataKeys.EntityFrameworkDatabaseTransactionWallet<TDbContext>());
    }

    internal static void AddEntityFrameworkDatabaseTransactionWallet<TDbContext>(
        this IAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData,
        IDbTransactionWallet transactionWallet)
        where TDbContext : DbContext
    {
        accelergreatEnvironmentPipelineData.Add(
            AccelergreatEnvironmentPipelineDataKeys.EntityFrameworkDatabaseTransactionWallet<TDbContext>(),
            transactionWallet);
    }

    public static IDbContextFactory<TDbContext> GetEntityFrameworkDbContextFactory<TDbContext>(
        this IReadOnlyAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData)
        where TDbContext : DbContext
    {
        return accelergreatEnvironmentPipelineData.Get<IDbContextFactory<TDbContext>>(
            AccelergreatEnvironmentPipelineDataKeys.EntityFrameworkDbContextFactory<TDbContext>());
    }

    internal static void AddEntityFrameworkDbContextFactory<TDbContext>(
        this IAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData,
        IDbContextFactory<TDbContext> dbContextFactory)
        where TDbContext : DbContext
    {
        accelergreatEnvironmentPipelineData.Add(
            AccelergreatEnvironmentPipelineDataKeys.EntityFrameworkDbContextFactory<TDbContext>(),
            dbContextFactory);
    }
}