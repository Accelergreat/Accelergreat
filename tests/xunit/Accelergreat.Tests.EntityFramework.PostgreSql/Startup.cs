using Accelergreat.Tests.EntityFramework.PostgreSql.Components;
using Accelergreat.Xunit;

namespace Accelergreat.Tests.EntityFramework.PostgreSql;

public class Startup : IAccelergreatStartup
{
    public void Configure(IAccelergreatBuilder builder)
    {
        builder.AddAccelergreatComponent<TestPostgreSqlDatabaseComponent>();
    }
}