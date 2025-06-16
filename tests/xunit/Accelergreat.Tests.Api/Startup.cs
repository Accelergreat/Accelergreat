using Accelergreat.Tests.Api.Components;
using Accelergreat.Xunit;

namespace Accelergreat.Tests.Api;

internal class Startup : IAccelergreatStartup
{
    public void Configure(IAccelergreatBuilder builder)
    {
        builder.AddAccelergreatComponent<TestSqlServerDatabaseComponent>();
        builder.AddAccelergreatComponent<TestApiWebAppComponent>();
        builder.AddAccelergreatComponent<TestApiWebAppComponent2>();
    }
}