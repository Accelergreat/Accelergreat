using Accelergreat.EntityFramework.SqlServer;
using Accelergreat.Tests.EntityFramework.Relational.AdventureWorks;
using Microsoft.Extensions.Configuration;

namespace Accelergreat.Tests.EntityFramework.SqlServer.ServiceTransactions.Components;

public class TestSqlServerDatabaseComponent : SqlServerEntityFrameworkDatabaseComponent<AdventureWorks2016Context>
{
    public TestSqlServerDatabaseComponent(IConfiguration configuration) : base(configuration)
    {
    }
}