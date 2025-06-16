using Accelergreat.EntityFramework.SqlServer;
using Accelergreat.Tests.EntityFramework.Relational.AdventureWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace Accelergreat.Tests.Api.Components;

public class TestSqlServerDatabaseComponent : SqlServerEntityFrameworkDatabaseComponent<AdventureWorks2016Context>
{
    public TestSqlServerDatabaseComponent(IConfiguration configuration) : base(configuration)
    {
    }

    protected override void ConfigureDbContextOptions(SqlServerDbContextOptionsBuilder options)
    {
        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
    }
}