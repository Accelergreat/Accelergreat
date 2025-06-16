using Accelergreat.Components;
using Accelergreat.EntityFramework.Extensions;
using Accelergreat.EntityFramework.SqlServer.SnapshotRollback;
using Accelergreat.EntityFramework.SqlServer.Transactions;
using Accelergreat.Environments;
using Accelergreat.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace Accelergreat.EntityFramework.SqlServer;

/// <summary>
/// A component for an Sql Server database managed via Entity Framework.
/// <para>The following objects are published to the environment pipeline data when using <b>SnapshotRollback</b> reset strategy:</para>
/// <para>- Type: <see cref="string"/>. Key: <see cref="AccelergreatEnvironmentPipelineDataKeys.EntityFrameworkDatabaseConnectionString{TContext}"/>.</para>
/// <para>The following objects are published to the environment pipeline data when using <b>Transactions</b> reset strategy:</para>
/// <para>- Type: <see cref="SqlConnection"/>. Key: <see cref="AccelergreatEnvironmentPipelineDataKeys.EntityFrameworkDatabaseConnection{TContext}"/>.</para>
/// <para>- Types: <see cref="IDbTransactionWallet"/>, <see cref="IDbTransactionWallet{SqlTransaction}"/>. Key: <see cref="AccelergreatEnvironmentPipelineDataKeys.EntityFrameworkDatabaseTransactionWallet{TDbContext}"/>.</para>
/// <para>See <see cref="SqlServerEntityFrameworkConfiguration"/> for default configuration values. You can override these in your Accelergreat configuration.</para>
/// </summary>
public class SqlServerEntityFrameworkDatabaseComponent<TDbContext>
    : EntityFrameworkDatabaseComponentBase<TDbContext>, IEntityFrameworkDatabaseComponent<TDbContext>
    where TDbContext : DbContext
{
    private readonly IEntityFrameworkDatabaseComponent<TDbContext> _databaseComponent;
    private bool _isDisposed;

    // ReSharper disable once MemberCanBeProtected.Global
    public SqlServerEntityFrameworkDatabaseComponent(IConfiguration configuration)
    {
        var sqlServerEntityFrameworkConfiguration = SqlServerEntityFrameworkConfigurationProvider.GetConfiguration(configuration);

        var databaseName = DatabaseNameFactory.NewDatabaseName<TDbContext>();

        _databaseComponent = sqlServerEntityFrameworkConfiguration.ResetStrategy switch
        {
            SqlSeverResetStrategy.SnapshotRollback =>
                new SnapshotRollbackSqlServerEntityFrameworkDatabaseComponent<TDbContext>(
                    sqlServerEntityFrameworkConfiguration,
                    databaseName,
                    OnDatabaseInitializedCoreAsync,
                    ConfigureDbContextOptions,
                    OnInitializing),
            SqlSeverResetStrategy.Transactions =>
                new TransactionSqlServerEntityFrameworkDatabaseComponent<TDbContext>(
                    sqlServerEntityFrameworkConfiguration,
                    databaseName,
                    OnDatabaseInitializedCoreAsync,
                    ConfigureDbContextOptions,
                    OnInitializing),
            _ =>
                throw new AccelergreatConfigurationException(SqlServerEntityFrameworkConfiguration.ConfigurationSectionName, nameof(SqlServerEntityFrameworkConfiguration.ResetStrategy), "value is invalid")
        };
    }

    /// <summary>
    /// Call before the component is initialized.
    /// <para>Override this method to set any values on <paramref name="configuration"/> at runtime.</para>
    /// </summary>
    protected virtual void OnInitializing(IReadOnlyAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData, SqlServerEntityFrameworkConfiguration configuration)
    {
    }

    /// <summary>
    /// Called after the database has been created.
    /// <para>Override this method to persist any changes to the database that will be globally available to all tests.</para>
    /// </summary>
    protected virtual Task OnDatabaseInitializedAsync(TDbContext context)
    {
        return Task.CompletedTask;
    }

    private async Task OnDatabaseInitializedCoreAsync(TDbContext context)
    {
        DbContextFactory = _databaseComponent.DbContextFactory;

        await OnDatabaseInitializedAsync(context);

        LoadGlobalDataItems(context);
    }

    /// <summary>
    /// Called after the database has been reset.
    /// <para>Override this method to persist any changes to the database that will be globally available to all tests.</para>
    /// <para>Useful for when you have global database changes that don't persist when reset.</para>
    /// <para>Most of the time you should persist global database changes via <see cref="OnDatabaseInitializedAsync"/>.</para>
    /// </summary>
    protected virtual Task OnDatabaseResetAsync(TDbContext context)
    {
        return Task.CompletedTask;
    }

    protected virtual void ConfigureDbContextOptions(SqlServerDbContextOptionsBuilder options)
    {
    }

    async Task IAccelergreatComponent.InitializeAsync(IAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData)
    {
        await _databaseComponent.InitializeAsync(accelergreatEnvironmentPipelineData);

        accelergreatEnvironmentPipelineData.AddEntityFrameworkDbContextFactory(DbContextFactory);
    }

    async Task IAccelergreatComponent.ResetAsync()
    {
        await _databaseComponent.ResetAsync();

        await using var context = DbContextFactory.NewDbContext();

        await OnDatabaseResetAsync(context);
    }

    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (_isDisposed)
        {
            return;
        }

        if (disposing)
        {
            await _databaseComponent.DisposeAsync();
        }

        _isDisposed = true;
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await DisposeAsync(true);

        GC.SuppressFinalize(this);
    }
}