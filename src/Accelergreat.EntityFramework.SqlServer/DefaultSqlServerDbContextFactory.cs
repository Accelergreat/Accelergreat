using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Accelergreat.EntityFramework.SqlServer;


internal sealed class DefaultSqlServerDbContextFactory<TDbContext>
    : DbContextFactoryBase<TDbContext>, IDbContextFactory<TDbContext>
    where TDbContext : DbContext
{
    private readonly string _connectionString;
    private readonly Action<SqlServerDbContextOptionsBuilder>? _sqlServerOptionsAction;

    internal DefaultSqlServerDbContextFactory(
        string connectionString,
        Action<SqlServerDbContextOptionsBuilder>? sqlServerOptionsAction)
    {
        _connectionString = connectionString;
        _sqlServerOptionsAction = sqlServerOptionsAction;
    }

    internal override void ConfigureOptions(DbContextOptionsBuilder<TDbContext> optionsBuilder)
    {
#if NET9_0_OR_GREATER
        optionsBuilder.ConfigureWarnings(config => config.Ignore(RelationalEventId.PendingModelChangesWarning));
#endif
        optionsBuilder.UseSqlServer(_connectionString, _sqlServerOptionsAction);
    }

    IDbContextFactory<TDbContext2> IDbContextFactory<TDbContext>.ChangeContextType<TDbContext2>()
    {
        return new DefaultSqlServerDbContextFactory<TDbContext2>(_connectionString, _sqlServerOptionsAction);
    }
}