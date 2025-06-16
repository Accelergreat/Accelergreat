using System.ComponentModel;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;

#pragma warning disable EF1001 // Internal EF Core API usage.

namespace Accelergreat.EntityFramework.SqlServer.Transactions.Overrides;


internal sealed class AccelergreatSqlServerTransactionOverride : SqlServerTransaction
{
    private readonly Stack<string> _savepointNames;

#if NET6_0_OR_GREATER

    public AccelergreatSqlServerTransactionOverride(
        IRelationalConnection connection,
        DbTransaction transaction,
        Guid transactionId,
        IDiagnosticsLogger<DbLoggerCategory.Database.Transaction> logger,
        bool transactionOwned,
        ISqlGenerationHelper sqlGenerationHelper)
        : base(connection, transaction, transactionId, logger, transactionOwned, sqlGenerationHelper)
    {
        _savepointNames = new Stack<string>();

        CreateNextSavepoint();
    }

#elif NETCOREAPP3_1_OR_GREATER

        public AccelergreatSqlServerTransactionOverride(
            IRelationalConnection connection, 
            DbTransaction transaction, 
            Guid transactionId, 
            IDiagnosticsLogger<DbLoggerCategory.Database.Transaction> logger, 
            bool transactionOwned) 
            : base(connection, transaction, transactionId, logger, transactionOwned)
        {
            _savepointNames = new Stack<string>();

            CreateNextSavepoint();
        }

#endif

    public override void Commit()
    {
        CreateNextSavepoint();
    }

    public override Task CommitAsync(CancellationToken cancellationToken = new())
    {
        return CreateNextSavepointAsync();
    }

    public override void Rollback()
    {
        TryRollbackToPreviousSavepoint();
    }

    public override Task RollbackAsync(CancellationToken cancellationToken = new())
    {
        return TryRollbackToPreviousSavepointAsync();
    }

    protected override void ClearTransaction()
    {
        // Do nothing because in a test context we do not want to remove the transaction from the connection.
    }

    protected override Task ClearTransactionAsync(CancellationToken cancellationToken = new())
    {
        return Task.CompletedTask;
    }

    private void CreateNextSavepoint()
    {
        var savepointName = GetNextSavepointName();

        _savepointNames.Push(savepointName);

        CreateSavepoint(savepointName);
    }

    private Task CreateNextSavepointAsync()
    {
        var savepointName = GetNextSavepointName();

        _savepointNames.Push(savepointName);

        return CreateSavepointAsync(savepointName);
    }

    private void TryRollbackToPreviousSavepoint()
    {
        if (_savepointNames.TryPop(out var savepoint))
        {
            RollbackToSavepoint(savepoint);
        }
    }

    private async Task TryRollbackToPreviousSavepointAsync()
    {
        if (_savepointNames.TryPop(out var savepoint))
        {
            await RollbackToSavepointAsync(savepoint);
        }
    }

    private static string GetNextSavepointName()
    {
        return string.Concat(Guid.NewGuid().ToString("N").Select(c => (char)(c + 17)))[..10];
    }
}

#pragma warning restore EF1001 // Internal EF Core API usage.