using System.ComponentModel;
using System.Data.Common;
using Accelergreat.EntityFramework.Exceptions;

namespace Accelergreat.EntityFramework;


internal sealed class DbTransactionWallet<TDbTransaction> :
    IDbTransactionWallet<TDbTransaction>,
    IDbTransactionWallet,
    IAsyncDisposable 
    where TDbTransaction : DbTransaction
{
    internal TDbTransaction? Transaction { get; private set; }

    internal void SetTransaction(TDbTransaction transaction)
    {
        if (Transaction is not null)
        {
            throw new AccelergreatDbTransactionWalletException();
        }

        Transaction = transaction;
    }

    internal async ValueTask DisposeAsync()
    {
        if (Transaction is not null)
        {
            await Transaction.DisposeAsync();
            Transaction = null;
        }
    }

    TDbTransaction? IDbTransactionWallet<TDbTransaction>.Transaction => Transaction;
    DbTransaction? IDbTransactionWallet.Transaction => Transaction;
    ValueTask IAsyncDisposable.DisposeAsync() => DisposeAsync();
}