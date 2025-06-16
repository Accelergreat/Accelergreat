using System.ComponentModel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
// ReSharper disable ClassNeverInstantiated.Global

#pragma warning disable EF1001 // Internal EF Core API usage.

namespace Accelergreat.EntityFramework.SqlServer.Transactions.Overrides;


internal sealed class AccelergreatSqlServerTransactionDbContextFactoryOverride<TDbContext> : DbContextFactory<TDbContext>
    where TDbContext : DbContext
{
    private readonly IDbTransactionWallet<SqlTransaction> _transactionWallet;

    public AccelergreatSqlServerTransactionDbContextFactoryOverride(
        IServiceProvider serviceProvider,
        DbContextOptions<TDbContext> options, 
        IDbContextFactorySource<TDbContext> factorySource, 
        IDbTransactionWallet<SqlTransaction> transactionWallet) 
        : base(serviceProvider, options, factorySource)
    {
        _transactionWallet = transactionWallet;
    }

    public override TDbContext CreateDbContext()
    {
        var context = base.CreateDbContext();

        context.Database.UseTransaction(_transactionWallet.Transaction!);

        return context;
    }
}

#pragma warning restore EF1001 // Internal EF Core API usage.