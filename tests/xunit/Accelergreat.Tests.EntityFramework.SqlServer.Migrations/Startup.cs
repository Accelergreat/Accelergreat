using Accelergreat.Tests.EntityFramework.SqlServer.Migrations.Components;
using Accelergreat.Xunit;

namespace Accelergreat.Tests.EntityFramework.SqlServer.Migrations;

public class Startup : IAccelergreatStartup
{
    public void Configure(IAccelergreatBuilder builder)
    {
        builder.AddAccelergreatComponent<TestSqlServerDatabaseComponent>();
    }
}