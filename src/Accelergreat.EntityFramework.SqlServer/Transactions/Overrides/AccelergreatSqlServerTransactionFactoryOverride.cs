using System.ComponentModel;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;

#pragma warning disable EF1001 // Internal EF Core API usage.

namespace Accelergreat.EntityFramework.SqlServer.Transactions.Overrides;


internal sealed class AccelergreatSqlServerTransactionFactoryOverride : SqlServerTransactionFactory
{
#if NET6_0_OR_GREATER
    public AccelergreatSqlServerTransactionFactoryOverride(RelationalTransactionFactoryDependencies dependencies) : base(dependencies)
    {
    }
#endif

    public override RelationalTransaction Create(
        IRelationalConnection connection, 
        DbTransaction transaction, 
        Guid transactionId,
        IDiagnosticsLogger<DbLoggerCategory.Database.Transaction> logger, 
        bool transactionOwned)
    {
#if NET6_0_OR_GREATER
        return new AccelergreatSqlServerTransactionOverride(
            connection, 
            transaction, 
            transactionId, 
            logger,
            transactionOwned, 
            Dependencies.SqlGenerationHelper);
#else
            return new AccelergreatSqlServerTransactionOverride(
                connection, 
                transaction, 
                transactionId, 
                logger,
                transactionOwned);
#endif
    }
}

#pragma warning restore EF1001 // Internal EF Core API usage.