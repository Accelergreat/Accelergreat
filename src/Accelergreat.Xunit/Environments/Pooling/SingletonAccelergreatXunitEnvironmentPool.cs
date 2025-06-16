using System.ComponentModel;
using Accelergreat.Environments;
using Accelergreat.Environments.Pooling;
using Microsoft.Extensions.Logging;

namespace Accelergreat.Xunit.Environments.Pooling;


internal sealed class SingletonAccelergreatXunitEnvironmentPool : SingletonAccelergreatEnvironmentPool, IAccelergreatInitialize
{
    public SingletonAccelergreatXunitEnvironmentPool(
        IAccelergreatEnvironment environment,
        ILogger<SingletonAccelergreatEnvironmentPool> logger) 
        : base(environment, logger)
    {
    }

    Task IAccelergreatInitialize.InitializeAsync()
    {
        return InitializeAsync();
    }
}