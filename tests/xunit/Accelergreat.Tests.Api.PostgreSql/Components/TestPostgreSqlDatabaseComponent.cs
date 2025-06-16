using Accelergreat.EntityFramework.PostgreSql;
using Accelergreat.Tests.EntityFramework.Relational.AdventureWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace Accelergreat.Tests.Api.PostgreSql.Components;

public class TestPostgreSqlDatabaseComponent : PostgreSqlEntityFrameworkDatabaseComponent<AdventureWorks2016Context>
{
    public TestPostgreSqlDatabaseComponent(IConfiguration configuration) : base(configuration)
    {
    }

    protected override void ConfigureDbContextOptions(NpgsqlDbContextOptionsBuilder options)
    {
        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
    }
}