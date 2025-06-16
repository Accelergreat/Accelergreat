using Accelergreat.Tests.EntityFramework.PostgreSql.ServiceTransactions.Components;
using Accelergreat.Xunit;

namespace Accelergreat.Tests.EntityFramework.PostgreSql.ServiceTransactions;

public class Startup : IAccelergreatStartup
{
    public void Configure(IAccelergreatBuilder builder)
    {
        builder.AddAccelergreatComponent<TestPostgreSqlDatabaseComponent>();
    }
}