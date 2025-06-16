using Accelergreat.EntityFramework.SqlServer;
using Accelergreat.Tests.EntityFramework.SqlServer.MigrationsDatabase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace Accelergreat.Tests.EntityFramework.SqlServer.Migrations.Components;

public class TestSqlServerDatabaseComponent : SqlServerEntityFrameworkDatabaseComponent<TestMigrationsDbContext>
{
    public TestSqlServerDatabaseComponent(IConfiguration configuration) : base(configuration)
    {
    }

    protected override void ConfigureDbContextOptions(SqlServerDbContextOptionsBuilder options)
    {
        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
    }
}