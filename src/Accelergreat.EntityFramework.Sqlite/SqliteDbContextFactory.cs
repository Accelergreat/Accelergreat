using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Accelergreat.EntityFramework.Sqlite;

internal sealed class SqliteDbContextFactory<TDbContext> 
    : DbContextFactoryBase<TDbContext>, IDbContextFactory<TDbContext>
    where TDbContext : DbContext
{
    private readonly string _connectionString;
    private readonly Action<SqliteDbContextOptionsBuilder>? _sqliteOptionsAction;

    internal SqliteDbContextFactory(
        string connectionString, 
        Action<SqliteDbContextOptionsBuilder>? sqliteOptionsAction)
    {
        _connectionString = connectionString;
        _sqliteOptionsAction = sqliteOptionsAction;
    }

    internal override void ConfigureOptions(DbContextOptionsBuilder<TDbContext> optionsBuilder)
    {
        optionsBuilder.UseSqlite(_connectionString, _sqliteOptionsAction);
    }

    IDbContextFactory<TDbContext2> IDbContextFactory<TDbContext>.ChangeContextType<TDbContext2>()
    {
        return new SqliteDbContextFactory<TDbContext2>(_connectionString, _sqliteOptionsAction);
    }
}