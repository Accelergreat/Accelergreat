using System.ComponentModel;
using Accelergreat.EntityFramework.SqlServer.Transactions.Overrides;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace Accelergreat.EntityFramework.SqlServer.Transactions;


internal sealed class TransactionSqlServerDbContextFactory<TDbContext> 
    : TransactionalDbContextFactoryBase<TDbContext>, IDbContextFactory<TDbContext>
    where TDbContext : DbContext
{
    private readonly SqlConnection _connection;
    private readonly IDbTransactionWallet<SqlTransaction> _transactionWallet;
    private readonly Action<SqlServerDbContextOptionsBuilder>? _sqlServerOptionsAction;

    internal TransactionSqlServerDbContextFactory(
        SqlConnection connection,
        IDbTransactionWallet<SqlTransaction> transactionWallet, 
        Action<SqlServerDbContextOptionsBuilder>? sqlServerOptionsAction)
    {
        _connection = connection;
        _transactionWallet = transactionWallet;
        _sqlServerOptionsAction = sqlServerOptionsAction;
    }

    internal override void ConfigureOptions(DbContextOptionsBuilder<TDbContext> optionsBuilder)
    {
#if NET9_0_OR_GREATER
        optionsBuilder.ConfigureWarnings(config => config.Ignore(RelationalEventId.PendingModelChangesWarning));
#endif

        optionsBuilder.UseSqlServer(_connection, sqlServerOptionsBuilder =>
        {
            _sqlServerOptionsAction?.Invoke(sqlServerOptionsBuilder);
        });
    }

    internal override void SetupTransactionOverriding(DbContextOptionsBuilder<TDbContext> optionsBuilder)
    {
#pragma warning disable EF1001 // Internal EF Core API usage.
        optionsBuilder
            .ReplaceService<ISqlServerConnection, AccelergreatSqlServerTransactionOverrideConnectionOverride>()
            .ReplaceService<IRelationalTransactionFactory, AccelergreatSqlServerTransactionFactoryOverride>();
#pragma warning restore EF1001 // Internal EF Core API usage.
    }

    protected override void ConfigureContext(TDbContext context)
    {
        if (_transactionWallet.Transaction is { } transaction)
        {
            context.Database.UseTransaction(transaction);
        }
    }

    IDbContextFactory<TDbContext2> IDbContextFactory<TDbContext>.ChangeContextType<TDbContext2>()
    {
        return new TransactionSqlServerDbContextFactory<TDbContext2>(
            _connection, 
            _transactionWallet,
            _sqlServerOptionsAction);
    }
}