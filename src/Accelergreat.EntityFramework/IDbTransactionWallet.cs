using System.Data.Common;

namespace Accelergreat.EntityFramework;

public interface IDbTransactionWallet
{
    DbTransaction? Transaction { get; }
}

public interface IDbTransactionWallet<out TDbTransaction>
    where TDbTransaction : DbTransaction
{
    TDbTransaction? Transaction { get; }
}