using System.ComponentModel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

#pragma warning disable EF1001 // Internal EF Core API usage.

namespace Accelergreat.EntityFramework.SqlServer.Transactions.Overrides;


internal sealed class AccelergreatSqlServerTransactionDbContextFactorySourceOverride<TDbContext> : DbContextFactorySource<TDbContext>
    where TDbContext : DbContext
{
    private readonly IDbTransactionWallet<SqlTransaction> _transactionWallet;

    public AccelergreatSqlServerTransactionDbContextFactorySourceOverride(IDbTransactionWallet<SqlTransaction> transactionWallet)
    {
        _transactionWallet = transactionWallet;
    }

    public override Func<IServiceProvider, DbContextOptions<TDbContext>, TDbContext> Factory => Create;

    private TDbContext Create(IServiceProvider serviceProvider, DbContextOptions<TDbContext> options)
    {
        var context = base.Factory(serviceProvider, options);

        context.Database.UseTransaction(_transactionWallet.Transaction!);

        return context;
    }
}

#pragma warning restore EF1001 // Internal EF Core API usage.