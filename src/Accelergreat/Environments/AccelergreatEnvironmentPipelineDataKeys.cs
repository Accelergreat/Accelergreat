namespace Accelergreat.Environments;

public static class AccelergreatEnvironmentPipelineDataKeys
{
    public const string EnvironmentId = "EnvironmentId";

    public static string EntityFrameworkDatabaseConnectionString<TDbContext>()
        => $"EntityFrameworkDatabaseConnectionString_{typeof(TDbContext).Name}";

    public static string EntityFrameworkDatabaseConnection<TDbContext>()
        => $"EntityFrameworkDatabaseConnection_{typeof(TDbContext).Name}";

    public static string EntityFrameworkDatabaseTransactionWallet<TDbContext>()
        => $"EntityFrameworkDatabaseConnectionTransactionWallet_{typeof(TDbContext).Name}";

    public static string EntityFrameworkDatabaseName<TDbContext>()
        => $"EntityFrameworkDatabaseName_{typeof(TDbContext).Name}";

    public static string EntityFrameworkInMemoryDatabaseRoot<TDbContext>()
        => $"EntityFrameworkInMemoryDatabaseRoot_{typeof(TDbContext).Name}";

    public static string EntityFrameworkDbContextFactory<TDbContext>()
        => $"EntityFrameworkDbContextFactory_{typeof(TDbContext).Name}";

    public static string WebAppHttpClient<TEntryPoint>()
        => $"WebAppHttpClient_{typeof(TEntryPoint).AssemblyQualifiedName}";

    public static string KestrelWebAppHttpBaseAddress<TEntryPoint>()
        => $"KestrelWebAppHttpBaseAddress_{typeof(TEntryPoint).AssemblyQualifiedName}";
}