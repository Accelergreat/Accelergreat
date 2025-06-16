using Accelergreat.Tests.EntityFramework.SqlServer.ServiceTransactions.Components;
using Accelergreat.Xunit;

namespace Accelergreat.Tests.EntityFramework.SqlServer.ServiceTransactions;

public class Startup : IAccelergreatStartup
{
    public void Configure(IAccelergreatBuilder builder)
    {
        builder.AddAccelergreatComponent<TestSqlServerDatabaseComponent>();
    }
}