using Xunit;

namespace Accelergreat.Xunit;

public abstract partial class AccelergreatXunitTest : IAsyncLifetime
{
    async Task IAsyncLifetime.InitializeAsync()
    {
        await InitializeEnvironmentAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await DisposeEnvironmentAsync();
    }
}
