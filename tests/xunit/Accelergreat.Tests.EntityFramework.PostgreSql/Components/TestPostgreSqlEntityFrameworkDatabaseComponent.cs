using Accelergreat.EntityFramework.PostgreSql;
using Accelergreat.Tests.EntityFramework.Relational.AdventureWorks;
using Microsoft.Extensions.Configuration;

namespace Accelergreat.Tests.EntityFramework.PostgreSql.Components;

public class TestPostgreSqlDatabaseComponent : PostgreSqlEntityFrameworkDatabaseComponent<AdventureWorks2016Context>
{
    public TestPostgreSqlDatabaseComponent(IConfiguration configuration) : base(configuration)
    {
    }
}