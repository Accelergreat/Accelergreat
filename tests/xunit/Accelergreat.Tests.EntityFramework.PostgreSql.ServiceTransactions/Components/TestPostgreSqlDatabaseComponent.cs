using Accelergreat.EntityFramework.PostgreSql;
using Accelergreat.Tests.EntityFramework.Relational.AdventureWorks;
using Microsoft.Extensions.Configuration;

namespace Accelergreat.Tests.EntityFramework.PostgreSql.ServiceTransactions.Components;

public class TestPostgreSqlDatabaseComponent : PostgreSqlEntityFrameworkDatabaseComponent<AdventureWorks2016Context>
{
    public TestPostgreSqlDatabaseComponent(IConfiguration configuration) : base(configuration)
    {
    }
}