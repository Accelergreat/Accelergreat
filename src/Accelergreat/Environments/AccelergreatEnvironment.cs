using System.Diagnostics;
using Accelergreat.Components;
using Accelergreat.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Accelergreat.Environments;

internal sealed class AccelergreatEnvironment : IAccelergreatEnvironment
{
    private readonly int _environmentId;
    private readonly IDictionary<Type, IAccelergreatComponent> _components;
    private readonly ILogger<AccelergreatEnvironment> _logger;
    private readonly Stopwatch _stopwatch;
    private int _allocationCount;
    private bool _isInitialized;
    private bool _isDisposed;

    public AccelergreatEnvironment(
        IAccelergreatEnvironmentIdAllocator accelergreatEnvironmentIdAllocator, 
        IServiceProvider serviceProvider,
        ILogger<AccelergreatEnvironment> logger)
    {
        _environmentId = accelergreatEnvironmentIdAllocator.Allocate();

        _components = serviceProvider
            .GetServices(typeof(IAccelergreatComponent))
            .Cast<IAccelergreatComponent>()
            .ToDictionary(
                component => component is IAccelergreatComponentDecorator decorator
                    ? decorator.GetComponent().GetType()
                    : component.GetType(),
                component => component);

        _logger = logger;

        _stopwatch = new Stopwatch();

        _allocationCount = 0;
    }

    async Task IAccelergreatEnvironment.InitializeAsync()
    {
        if (!_isInitialized)
        {
            _stopwatch.Restart();

            var accelergreatEnvironmentPipelineData = new AccelergreatEnvironmentPipelineData();

            accelergreatEnvironmentPipelineData.AddEnvironmentId(_environmentId);

            foreach (var component in _components.Values)
            {
                await component.InitializeAsync(accelergreatEnvironmentPipelineData);
            }

            _stopwatch.Stop();

            _logger.LogInformation("Initialized environment [{environmentId}] in {elapsedMilliseconds}ms", _environmentId, _stopwatch.ElapsedMilliseconds);

            _isInitialized = true;
        }
    }

    TAccelergreatComponent IAccelergreatEnvironment.GetComponent<TAccelergreatComponent>()
    {
        TAccelergreatComponent result;

        var component = _components[typeof(TAccelergreatComponent)];

        if (component is IAccelergreatComponentDecorator decorator)
        {
            result = (TAccelergreatComponent)decorator.GetComponent();
        }
        else
        {
            result = (TAccelergreatComponent)component;
        }

        return result;
    }

    TInterface IAccelergreatEnvironment.GetComponentByInterface<TInterface>()
    {
        TInterface result;

        var component = _components.Single(x => typeof(TInterface).IsAssignableFrom(x.Key)).Value;

        if (component is IAccelergreatComponentDecorator decorator)
        {
            result = (TInterface)decorator.GetComponent();
        }
        else
        {
            result = (TInterface)component;
        }

        return result;
    }

    int IAccelergreatEnvironment.EnvironmentId => _environmentId;

    int IAccelergreatEnvironment.AllocationCount => _allocationCount;

    void IAccelergreatEnvironment.IncrementAllocationCount()
    {
        _allocationCount++;
    }

    async Task IAccelergreatEnvironment.ResetAsync()
    {
        _stopwatch.Restart();

        var resetComponentTasks = _components.Values
            .Select(x => x.ResetAsync())
            .ToArray();

        await Task.WhenAll(resetComponentTasks);

        _stopwatch.Stop();

        _logger.LogInformation("Reset environment [{environmentId}] in {elapsedMilliseconds}ms.", _environmentId, _stopwatch.ElapsedMilliseconds);
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (!_isDisposed)
        {
            _isDisposed = true;

            foreach (var component in _components.Values
                         .Where(x =>
                             x is not IAccelergreatComponentDecorator decorator || !decorator.IsSingleton())
                         .Reverse())
            {
                await component.DisposeAsync();
            }
        }
    }
}