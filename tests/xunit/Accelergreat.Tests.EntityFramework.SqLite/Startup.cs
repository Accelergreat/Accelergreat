using Accelergreat.Tests.EntityFramework.SqLite.Components;
using Accelergreat.Xunit;

namespace Accelergreat.Tests.EntityFramework.SqLite;

public class Startup : IAccelergreatStartup
{
    public void Configure(IAccelergreatBuilder builder)
    {
        builder.AddAccelergreatComponent<TestSqliteSqlDatabaseComponent>();
    }
}