using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Accelergreat.Environments.Pooling;


internal abstract class SingletonAccelergreatEnvironmentPool
    : IAccelergreatEnvironmentPool, IAsyncDisposable, IDisposable
{
    private readonly IAccelergreatEnvironment _environment;
    private readonly ILogger<SingletonAccelergreatEnvironmentPool> _logger;
    private bool _isDisposed;

    private protected SingletonAccelergreatEnvironmentPool(
        IAccelergreatEnvironment environment,
        ILogger<SingletonAccelergreatEnvironmentPool> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    internal Task InitializeAsync()
    {

        return _environment.InitializeAsync();
    }


    async Task<IAccelergreatEnvironment> IAccelergreatEnvironmentPool.RentAsync()
    {
        if (_environment.AllocationCount > 0)
        {
            await _environment.ResetAsync();
        }

        _environment.IncrementAllocationCount();

        return _environment;
    }

    Task IAccelergreatEnvironmentPool.ReturnAsync(IAccelergreatEnvironment environment)
    {
        return Task.CompletedTask;
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

            _logger.LogInformation("Disposing environment");

            var stopwatch = new Stopwatch();

            stopwatch.Start();

            await _environment.DisposeAsync();

            _logger.LogInformation("Disposed environment in {elapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
        }
    }
}