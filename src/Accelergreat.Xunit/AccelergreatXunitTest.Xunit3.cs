using Xunit;

namespace Accelergreat.Xunit;

public abstract partial class AccelergreatXunitTest : IAsyncLifetime, IAsyncDisposable
{
    async ValueTask IAsyncLifetime.InitializeAsync()
    {
        await InitializeEnvironmentAsync();
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await DisposeEnvironmentAsync();
    }
}
