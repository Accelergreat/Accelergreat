using System.ComponentModel;
using Accelergreat.Components;
using Accelergreat.EntityFramework.Extensions;
using Accelergreat.Environments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Accelergreat.EntityFramework.SqlServer.SnapshotRollback;


internal sealed class SnapshotRollbackSqlServerEntityFrameworkDatabaseComponent<TDbContext>
    : IEntityFrameworkDatabaseComponent<TDbContext>
    where TDbContext : DbContext
{
    private readonly string _databaseName;
    private readonly string _snapshotName;
    private readonly SqlServerEntityFrameworkConfiguration _configuration;
    private readonly Func<TDbContext, Task> _preInitialStateSetTask;
    private readonly Action<SqlServerDbContextOptionsBuilder>? _sqlServerOptionsAction;
    private readonly Action<IReadOnlyAccelergreatEnvironmentPipelineData, SqlServerEntityFrameworkConfiguration> _onInitializingAction;
    private IDbContextFactory<TDbContext>? _masterDbContextFactory;
    private IDbContextFactory<TDbContext>? _dbContextFactory;
    private bool _isDisposed;

    internal SnapshotRollbackSqlServerEntityFrameworkDatabaseComponent(
        SqlServerEntityFrameworkConfiguration configuration,
        string databaseName,
        Func<TDbContext, Task> preInitialStateSetTask,
        Action<SqlServerDbContextOptionsBuilder>? sqlServerOptionsAction,
        Action<IReadOnlyAccelergreatEnvironmentPipelineData, SqlServerEntityFrameworkConfiguration> onInitializingAction)
    {
        _databaseName = databaseName;
        _snapshotName = $"{_databaseName}_init";
        _configuration = configuration;
        _preInitialStateSetTask = preInitialStateSetTask;
        _sqlServerOptionsAction = sqlServerOptionsAction;
        _onInitializingAction = onInitializingAction;
    }

    public IDbContextFactory<TDbContext> DbContextFactory =>
        _dbContextFactory ?? throw new NullReferenceException("Component has not been initialized yet.");

    async Task IAccelergreatComponent.InitializeAsync(IAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData)
    {
        _onInitializingAction(accelergreatEnvironmentPipelineData, _configuration);

        var connectionString = SqlServerConnectionStringProvider.GetConnectionString(_databaseName, _configuration);

        _dbContextFactory = new DefaultSqlServerDbContextFactory<TDbContext>(connectionString, _sqlServerOptionsAction);

        _masterDbContextFactory = new DefaultSqlServerDbContextFactory<TDbContext>(
            SqlServerConnectionStringProvider.GetConnectionString("master", _configuration),
            _sqlServerOptionsAction);

        await using var context = _dbContextFactory.NewDbContext();

        if (_configuration.CreateStrategy == SqlSeverCreateStrategy.Migrations)
        {
            await context.Database.MigrateAsync();
        }

        await context.Database.EnsureCreatedAsync();

        await _preInitialStateSetTask(context);

        var snapshotPath = Path.Combine(_configuration.SnapshotDirectory ?? Path.GetTempPath(), $"{_snapshotName}.ss");

        await context.Database.ExecuteSqlRawAsync($@"
CREATE DATABASE {_snapshotName} ON  
(
	NAME = {_databaseName}, 
	FILENAME = '{snapshotPath}' 
)  
AS SNAPSHOT OF {_databaseName}");

        accelergreatEnvironmentPipelineData.AddEntityFrameworkDatabaseName<TDbContext>(_databaseName);

        accelergreatEnvironmentPipelineData.AddEntityFrameworkDatabaseConnectionString<TDbContext>(connectionString);
    }

    async Task IAccelergreatComponent.ResetAsync()
    {
        if (_masterDbContextFactory is not null)
        {
            await using var masterContext = _masterDbContextFactory.NewDbContext();

            await masterContext.Database.ExecuteSqlRawAsync($@"
DECLARE @KillStatement NVARCHAR(MAX) = '';

SELECT
    @KillStatement = CONCAT(@KillStatement, 'KILL ', CAST([session_id] AS NVARCHAR(MAX)), ';')
FROM
    [sys].[dm_exec_sessions]
WHERE
    [database_id] = DB_ID('{_databaseName}')
	AND [session_id] != @@SPID
    AND [is_user_process] = 1;

IF LEN(@KillStatement) > 0 
BEGIN
	EXEC [sys].[sp_executesql] @KillStatement;
END;");

            await masterContext.Database.ExecuteSqlRawAsync($@"
RESTORE DATABASE {_databaseName} FROM   
DATABASE_SNAPSHOT = '{_snapshotName}'");
        }
    }

    private async ValueTask DisposeAsync(bool disposing)
    {
        if (_isDisposed)
        {
            return;
        }

        if (disposing && _dbContextFactory is not null)
        {
            await using var context = _dbContextFactory.NewDbContext();

            await context.Database.ExecuteSqlRawAsync($"DROP DATABASE {_snapshotName}");

            await context.Database.EnsureDeletedAsync();
        }

        _isDisposed = true;
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await DisposeAsync(true);
    }
}