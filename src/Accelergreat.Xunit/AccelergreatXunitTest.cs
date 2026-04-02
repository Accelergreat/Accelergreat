using Accelergreat.Components;
using Accelergreat.Environments;
using Accelergreat.Environments.Pooling;

namespace Accelergreat.Xunit;

/// <summary>
/// Base class for an Accelergreat Xunit tests
/// </summary>
public abstract partial class AccelergreatXunitTest
{
    private readonly IAccelergreatEnvironmentPool _environmentPool;
    private IAccelergreatEnvironment? _environment;

    protected AccelergreatXunitTest(IAccelergreatEnvironmentPool environmentPool)
    {
        _environmentPool = environmentPool;
    }

    /// <summary>
    /// Retrieve a <typeparamref name="TAccelergreatComponent"/>.
    /// </summary>
    protected TAccelergreatComponent GetComponent<TAccelergreatComponent>()
        where TAccelergreatComponent : IAccelergreatComponent
    {
        if (_environment is null)
        {
            throw new NullReferenceException("Environment has not yet been initialized");
        }

        return _environment.GetComponent<TAccelergreatComponent>();
    }

    /// <summary>
    /// Retrieve a <typeparamref name="TInterface"/>.
    /// </summary>
    protected TInterface GetComponentByInterface<TInterface>()
        where TInterface : notnull
    {
        if (_environment is null)
        {
            throw new NullReferenceException("Environment has not yet been initialized");
        }

        return _environment.GetComponentByInterface<TInterface>();
    }

    protected virtual Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    protected virtual Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    protected async Task InitializeEnvironmentAsync()
    {
        _environment = await _environmentPool.RentAsync();
        await InitializeAsync();
    }

    protected async Task DisposeEnvironmentAsync()
    {
        await DisposeAsync();

        if (_environment is not null)
        {
            await _environmentPool.ReturnAsync(_environment);
            _environment = null;
        }
    }
}