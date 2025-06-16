using Accelergreat.Components;
using Accelergreat.EntityFramework.Extensions;
using Accelergreat.Environments;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Accelergreat.EntityFramework.Sqlite;

/// <summary>
/// A component for a SQLite database managed via Entity Framework.
/// <para>The following objects are published to the environment pipeline data:</para>
/// <para>- Type: <see cref="string"/>. Key: <see cref="AccelergreatEnvironmentPipelineDataKeys.EntityFrameworkDatabaseConnectionString{TContext}"/>.</para>
/// </summary>
public class SqliteEntityFrameworkDatabaseComponent<TDbContext> : EntityFrameworkDatabaseComponentBase<TDbContext>, IEntityFrameworkDatabaseComponent<TDbContext>
    where TDbContext : DbContext
{
    private readonly string _databaseName;
    private readonly string _connectionString;
    private SqliteConnection? _keepAliveConnection;
    private bool _isDisposed;

    // ReSharper disable once MemberCanBeProtected.Global
    public SqliteEntityFrameworkDatabaseComponent()
    {
        _databaseName = DatabaseNameFactory.NewDatabaseName<TDbContext>();
        _connectionString =
            SqliteConnectionStringProvider.GetConnectionString(_databaseName);
        _keepAliveConnection = null;
        DbContextFactory = new SqliteDbContextFactory<TDbContext>(_connectionString, ConfigureDbContextOptions);

    }

    /// <summary>
    /// Called after the database has been created.
    /// <para>Override this method to persist any changes to the database that will be globally available to all tests.</para>
    /// </summary>
    protected virtual Task OnDatabaseInitializedAsync(TDbContext context)
    {
        return Task.CompletedTask;
    }

    protected virtual void ConfigureDbContextOptions(SqliteDbContextOptionsBuilder options)
    {
    }

    async Task IAccelergreatComponent.InitializeAsync(IAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData)
    {
        _keepAliveConnection = new SqliteConnection(_connectionString);

        await _keepAliveConnection.OpenAsync();

        await using var context = DbContextFactory.NewDbContext();

        await context.Database.EnsureCreatedAsync();

        await OnDatabaseInitializedAsync(context);

        LoadGlobalDataItems(context);

        accelergreatEnvironmentPipelineData.AddEntityFrameworkDatabaseName<TDbContext>(_databaseName);

        accelergreatEnvironmentPipelineData.AddEntityFrameworkDatabaseConnectionString<TDbContext>(_connectionString);

        accelergreatEnvironmentPipelineData.AddEntityFrameworkDbContextFactory(DbContextFactory);
    }

    async Task IAccelergreatComponent.ResetAsync()
    {
        _keepAliveConnection?.Close();

        _keepAliveConnection?.Dispose();

        _keepAliveConnection = new SqliteConnection(_connectionString);

        await _keepAliveConnection.OpenAsync();

        await using var context = DbContextFactory.NewDbContext();

        await context.Database.EnsureCreatedAsync();

        await OnDatabaseInitializedAsync(context);
    }

    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (_isDisposed)
        {
            return;
        }

        if (disposing && _keepAliveConnection is not null)
        {
            await _keepAliveConnection.CloseAsync();

            await _keepAliveConnection.DisposeAsync();
        }

        _isDisposed = true;
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await DisposeAsync(true);

        GC.SuppressFinalize(this);
    }
}