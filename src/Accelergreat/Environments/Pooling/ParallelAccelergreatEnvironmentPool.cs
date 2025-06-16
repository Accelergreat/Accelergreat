using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Accelergreat.Environments.Pooling;


internal abstract class ParallelAccelergreatEnvironmentPool 
    : IAccelergreatEnvironmentPool, IAsyncDisposable, IDisposable
{
    private readonly ILogger<ParallelAccelergreatEnvironmentPool> _logger;
    private readonly IReadOnlySet<IAccelergreatEnvironment> _environments;
    private readonly Channel<IAccelergreatEnvironment> _environmentChannel;
    private bool _isDisposed;

    private protected ParallelAccelergreatEnvironmentPool(
        IServiceProvider serviceProvider,
        ILogger<ParallelAccelergreatEnvironmentPool> logger,
        int testCollectionCount,
        int? maxEnvironmentCount)
    {
        var environmentCount = maxEnvironmentCount.HasValue
            ? Math.Min(maxEnvironmentCount.Value, Math.Min(testCollectionCount, Environment.ProcessorCount))
            : Math.Min(testCollectionCount, Environment.ProcessorCount);
            
        var environments = new HashSet<IAccelergreatEnvironment>(environmentCount);
        
        for (var i = 0; i < environmentCount; i++)
        {
            environments.Add(serviceProvider.GetRequiredService<IAccelergreatEnvironment>());
        }
            
        _environments = environments;
        _environmentChannel = Channel.CreateBounded<IAccelergreatEnvironment>(environmentCount);
        _logger = logger;
    }

    protected async Task InitializeAsync()
    {
        _logger.LogInformation("Initializing {count} environments", _environments.Count);

        var environmentsInitializeStopwatch = new Stopwatch();

        environmentsInitializeStopwatch.Start();

        async Task InitializeEnvironment(IAccelergreatEnvironment environment)
        {
            await environment.InitializeAsync();

            await _environmentChannel.Writer.WriteAsync(environment);
        }

        var initializeEnvironmentTasks = _environments
            .Select(InitializeEnvironment)
            .ToArray();

        await Task.WhenAll(initializeEnvironmentTasks);

        environmentsInitializeStopwatch.Stop();

        _logger.LogInformation("Initialized {count} environments in {elapsedMilliseconds}ms", _environments.Count, environmentsInitializeStopwatch.ElapsedMilliseconds);
    }

    async Task<IAccelergreatEnvironment> IAccelergreatEnvironmentPool.RentAsync()
    {
        var environment = await _environmentChannel.Reader.ReadAsync();

        if (environment.AllocationCount > 0)
        {
            await environment.ResetAsync();
        }

        _logger.LogInformation("Environment [{environmentId}] allocated.", environment.EnvironmentId);

        environment.IncrementAllocationCount();

        return environment;
    }

    Task IAccelergreatEnvironmentPool.ReturnAsync(IAccelergreatEnvironment environment)
    {
        return _environmentChannel.Writer.WriteAsync(environment).AsTask();
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await DisposeAsync(true);

        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            DisposeAsync(true).GetAwaiter().GetResult();
        }
    }

    protected virtual async Task DisposeAsync(bool disposing)
    {
        if (!_isDisposed && disposing)
        {
            _isDisposed = true;

            _logger.LogInformation("Disposing {count} environments", _environments.Count);

            var stopwatch = new Stopwatch();

            stopwatch.Start();

            var disposeTasks = _environments
                .Select(x => x.DisposeAsync().AsTask())
                .ToArray();

            await Task.WhenAll(disposeTasks);

            stopwatch.Stop();

            _logger.LogInformation("Disposed {count} environments in {elapsedMilliseconds}ms", _environments.Count, stopwatch.ElapsedMilliseconds);
        }
    }
}