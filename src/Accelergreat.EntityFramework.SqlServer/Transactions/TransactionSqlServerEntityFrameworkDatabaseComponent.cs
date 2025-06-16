using System.ComponentModel;
using Accelergreat.Components;
using Accelergreat.EntityFramework.Extensions;
using Accelergreat.Environments;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Accelergreat.EntityFramework.SqlServer.Transactions;


internal sealed class TransactionSqlServerEntityFrameworkDatabaseComponent<TDbContext> : IEntityFrameworkDatabaseComponent<TDbContext>
    where TDbContext : DbContext
{
    private const string TransactionInitialStateSavePointName = "INITIAL_STATE";

    private readonly string _databaseName;
    private readonly DbTransactionWallet<SqlTransaction> _transactionWallet;
    private readonly SqlServerEntityFrameworkConfiguration _configuration;
    private readonly Func<TDbContext, Task> _preInitialStateSetTask;
    private readonly Action<SqlServerDbContextOptionsBuilder>? _sqlServerOptionsAction;
    private readonly Action<IReadOnlyAccelergreatEnvironmentPipelineData, SqlServerEntityFrameworkConfiguration> _onInitializingAction;
    private SqlConnection? _connection;
    private TransactionSqlServerDbContextFactory<TDbContext>? _transactionSqlServerDbContextFactory;
    private DefaultSqlServerDbContextFactory<TDbContext>? _defaultSqlServerDbContextFactory;
    private bool _isDisposed;

    internal TransactionSqlServerEntityFrameworkDatabaseComponent(
        SqlServerEntityFrameworkConfiguration configuration,
        string databaseName,
        Func<TDbContext, Task> preInitialStateSetTask,
        Action<SqlServerDbContextOptionsBuilder>? sqlServerOptionsAction,
        Action<IReadOnlyAccelergreatEnvironmentPipelineData, SqlServerEntityFrameworkConfiguration> onInitializingAction)
    {
        _databaseName = databaseName;
        _transactionWallet = new DbTransactionWallet<SqlTransaction>();
        _configuration = configuration;
        _preInitialStateSetTask = preInitialStateSetTask;
        _sqlServerOptionsAction = sqlServerOptionsAction;
        _onInitializingAction = onInitializingAction;
    }

    public IDbContextFactory<TDbContext> DbContextFactory =>
        _transactionSqlServerDbContextFactory ?? throw new NullReferenceException("Component has not been initialized yet.");

    async Task IAccelergreatComponent.InitializeAsync(IAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData)
    {
        _onInitializingAction(accelergreatEnvironmentPipelineData, _configuration);

        var connectionString = SqlServerConnectionStringProvider.GetConnectionString(_databaseName, _configuration);

        _connection = new SqlConnection(connectionString);

        _transactionSqlServerDbContextFactory = new TransactionSqlServerDbContextFactory<TDbContext>(_connection, _transactionWallet, _sqlServerOptionsAction);

        _defaultSqlServerDbContextFactory = new DefaultSqlServerDbContextFactory<TDbContext>(connectionString, _sqlServerOptionsAction);

        await using (var context = _defaultSqlServerDbContextFactory.NewDbContext())
        {
            if (_configuration.CreateStrategy == SqlSeverCreateStrategy.Migrations)
            {
                await context.Database.MigrateAsync();
            }

            await context.Database.EnsureCreatedAsync();

            await _preInitialStateSetTask(context);
        }

        await _connection.OpenAsync();

        _transactionWallet.SetTransaction(_connection.BeginTransaction());

        _transactionWallet.Transaction!.Save(TransactionInitialStateSavePointName);

        accelergreatEnvironmentPipelineData.AddEntityFrameworkDatabaseName<TDbContext>(_databaseName);

        accelergreatEnvironmentPipelineData.AddEntityFrameworkDatabaseConnectionString<TDbContext>(connectionString);

        accelergreatEnvironmentPipelineData.AddEntityFrameworkDatabaseConnection<TDbContext>(_connection);

        accelergreatEnvironmentPipelineData.AddEntityFrameworkDatabaseTransactionWallet<TDbContext>(_transactionWallet);
    }

    async Task IAccelergreatComponent.ResetAsync()
    {
        if (_connection is not null && _transactionWallet.Transaction is not null)
        {
            try
            {
                _transactionWallet.Transaction.Rollback(TransactionInitialStateSavePointName);
                _transactionWallet.Transaction.Save(TransactionInitialStateSavePointName);
            }
            catch (SqlException)
            {
                await _transactionWallet.DisposeAsync();

                _transactionWallet.SetTransaction(_connection.BeginTransaction());

                _transactionWallet.Transaction.Save(TransactionInitialStateSavePointName);
            }
        }
    }

    private async ValueTask DisposeAsync(bool disposing)
    {
        if (_isDisposed)
        {
            return;
        }

        if (disposing)
        {
            await _transactionWallet.DisposeAsync();

            if (_connection is not null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }

            if (_defaultSqlServerDbContextFactory is not null)
            {
                await using var context = _defaultSqlServerDbContextFactory.NewDbContext();

                await context.Database.EnsureDeletedAsync();
            }
        }

        _isDisposed = true;
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await DisposeAsync(true);
    }
}