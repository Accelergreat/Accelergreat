using Accelergreat.Environments.Pooling;
using Accelergreat.Xunit.Configuration;
using Microsoft.Extensions.Logging;
using Xunit.Sdk;

namespace Accelergreat.Xunit.Environments.Pooling;

internal sealed class ParallelAccelergreatXunitEnvironmentPool : ParallelAccelergreatEnvironmentPool, IAccelergreatInitialize
{
    public ParallelAccelergreatXunitEnvironmentPool(
        IServiceProvider serviceProvider,
        ILogger<ParallelAccelergreatXunitEnvironmentPool> logger,
        IAccelergreatXunitExecutionContext context)
        : base(serviceProvider, logger, context.TestCollectionCount, context.ExecutionOptions.MaxParallelThreads())
    {
    }

    Task IAccelergreatInitialize.InitializeAsync() => InitializeAsync();
}
