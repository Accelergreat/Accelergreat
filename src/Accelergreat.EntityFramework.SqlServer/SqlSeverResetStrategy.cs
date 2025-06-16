namespace Accelergreat.EntityFramework.SqlServer;

public enum SqlSeverResetStrategy : byte
{
    SnapshotRollback,
    Transactions
}