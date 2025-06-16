using Accelergreat.Environments;
using Microsoft.Extensions.DependencyInjection;

namespace Accelergreat.Components;

internal sealed class AccelergreatComponentDecorator : IAccelergreatComponentDecorator
{
    private readonly IAccelergreatComponent _innerComponent;
    private readonly ServiceLifetime _lifetime;
    private SemaphoreSlim? _executionSemaphoreSlim;

    internal AccelergreatComponentDecorator(IAccelergreatComponent component, ServiceLifetime lifetime)
    {
        _innerComponent = component;
        _lifetime = lifetime;
        _executionSemaphoreSlim = lifetime switch
        {
            ServiceLifetime.Singleton => new SemaphoreSlim(1, 1),
            _ => null
        };
    }

    IAccelergreatComponent IAccelergreatComponentDecorator.GetComponent()
    {
        return _innerComponent;
    }

    bool IAccelergreatComponentDecorator.IsSingleton()
    {
        return _lifetime == ServiceLifetime.Singleton;
    }

    async Task IAccelergreatComponent.InitializeAsync(IAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData)
    {
        if (_executionSemaphoreSlim is not null)
        {
            await _executionSemaphoreSlim.WaitAsync();
        }

        try
        {
            await _innerComponent.InitializeAsync(accelergreatEnvironmentPipelineData);
        }
        finally
        {
            _executionSemaphoreSlim?.Release();
        }
    }

    Task IAccelergreatComponent.ResetAsync()
    {
        return _lifetime switch
        {
            ServiceLifetime.Singleton => Task.CompletedTask,
            _ => _innerComponent.ResetAsync()
        };
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        try
        {
            if (_executionSemaphoreSlim is not null)
            {
                await _executionSemaphoreSlim.WaitAsync();
            }

            await _innerComponent.DisposeAsync();
        }
        catch (ObjectDisposedException exception) when (exception.Message.Contains("The semaphore has been disposed"))
        {
            // expected exception if _executionSemaphoreSlim has been disposed
        }
        finally
        {
            _executionSemaphoreSlim?.Dispose();

            _executionSemaphoreSlim = null;
        }
    }
}