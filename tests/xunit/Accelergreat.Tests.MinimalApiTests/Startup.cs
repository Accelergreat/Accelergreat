using Accelergreat.Tests.MinimalApiTests.Components;
using Accelergreat.Xunit;

namespace Accelergreat.Tests.MinimalApiTests;

internal class Startup : IAccelergreatStartup
{
    public void Configure(IAccelergreatBuilder builder)
    {
        builder.AddAccelergreatComponent<MinimalApiWebAppComponent>();
    }
}