using System.ComponentModel;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using IsolationLevel = System.Data.IsolationLevel;
// ReSharper disable ClassNeverInstantiated.Global

#pragma warning disable EF1001 // Internal EF Core API usage.

namespace Accelergreat.EntityFramework.SqlServer.Transactions.Overrides;


internal sealed class AccelergreatSqlServerTransactionOverrideConnectionOverride : SqlServerConnection
{
    public AccelergreatSqlServerTransactionOverrideConnectionOverride(RelationalConnectionDependencies dependencies) : base(dependencies)
    {
    }

    public override IDbContextTransaction BeginTransaction()
    {
        return BeginTransaction(IsolationLevel.Unspecified);
    }

    public override IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel)
    {
        return CurrentTransaction!;
    }

    public override Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = new())
    {
        return BeginTransactionAsync(IsolationLevel.Unspecified, cancellationToken);
    }

    public override Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel,
        CancellationToken cancellationToken = new())
    {
        return Task.FromResult(CurrentTransaction!);
    }
}

#pragma warning restore EF1001 // Internal EF Core API usage.