using System.Diagnostics;
using Accelergreat.Environments;
using Accelergreat.Environments.Pooling;
using Accelergreat.Xunit.Configuration;
using Accelergreat.Xunit.Environments.Pooling;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Sdk;
using Xunit.v3;

namespace Accelergreat.Xunit.Framework.Internal;

internal sealed class AccelergreatXunitTestAssemblyRunner : XunitTestAssemblyRunner
{
    private readonly ILogger _logger;
    private readonly IServiceCollection _serviceCollection;
    private readonly ServiceProvider _serviceProvider;
    private readonly Stopwatch _assemblyTestStopwatch;

    internal AccelergreatXunitTestAssemblyRunner(
        ILoggerProvider loggerProvider,
        IConfiguration configuration,
        IAccelergreatXunitExecutionContext executionContext,
        IAccelergreatStartup? startup)
    {
        _logger = loggerProvider.CreateLogger(string.Empty);
        _logger.LogInformation("Thank you for using Accelergreat. Your feedback is welcome.");

        _serviceCollection = new ServiceCollection();
        startup?.Configure(new AccelergreatBuilder(_serviceCollection, configuration));

        AddDefaultServices(configuration, loggerProvider, executionContext);
        _serviceProvider = _serviceCollection.BuildServiceProvider();
        _assemblyTestStopwatch = new Stopwatch();
    }

    protected override async ValueTask<bool> OnTestAssemblyStarting(XunitTestAssemblyRunnerContext ctxt)
    {
        await ctxt.Aggregator.RunAsync(async () =>
        {
            await InitializeSingletonServices(_serviceCollection, _serviceProvider);
        });

        _assemblyTestStopwatch.Start();
        return await base.OnTestAssemblyStarting(ctxt);
    }

    protected override async ValueTask<bool> OnTestAssemblyFinished(XunitTestAssemblyRunnerContext ctxt, RunSummary summary)
    {
        _assemblyTestStopwatch.Stop();
        _logger.LogInformation("Finished executing tests in {elapsedMilliseconds}ms", _assemblyTestStopwatch.ElapsedMilliseconds);

        await ctxt.Aggregator.RunAsync(async () =>
        {
            await _serviceProvider.DisposeAsync();
        });

        return await base.OnTestAssemblyFinished(ctxt, summary);
    }

    protected override ValueTask<RunSummary> RunTestCollection(
        XunitTestAssemblyRunnerContext ctxt,
        IXunitTestCollection testCollection,
        IReadOnlyCollection<IXunitTestCase> testCases)
    {
        var collectionRunner = new AccelergreatXunitTestCollectionRunner(_serviceCollection, _serviceProvider, _logger);

        return collectionRunner.Run(
            testCollection,
            testCases,
            ctxt.ExplicitOption,
            ctxt.MessageBus,
            ctxt.Aggregator.Clone(),
            ctxt.CancellationTokenSource,
            ctxt.ParallelMode,
            ctxt.Scheduler,
            ctxt.AssemblyFixtureMappings);
    }

    private void AddDefaultServices(
        IConfiguration configuration,
        ILoggerProvider loggerProvider,
        IAccelergreatXunitExecutionContext executionContext)
    {
        _serviceCollection.AddLogging(options => options.AddProvider(loggerProvider));
        _serviceCollection.AddSingleton(configuration);
        _serviceCollection.AddSingleton(executionContext);
        _serviceCollection.AddSingleton<IAccelergreatEnvironmentIdAllocator, AccelergreatEnvironmentIdAllocator>();
        _serviceCollection.AddTransient<IAccelergreatEnvironment, AccelergreatEnvironment>();

        if (CanExecuteParallel(executionContext))
        {
            _logger.LogInformation("Test execution strategy set to parallel.");
            _serviceCollection.AddSingleton<IAccelergreatEnvironmentPool, ParallelAccelergreatXunitEnvironmentPool>();
        }
        else
        {
            _logger.LogInformation("Test execution strategy set to sequential.");
            _serviceCollection.AddSingleton<IAccelergreatEnvironmentPool, SingletonAccelergreatXunitEnvironmentPool>();
        }
    }

    private static bool CanExecuteParallel(IAccelergreatXunitExecutionContext executionContext)
    {
        if (executionContext.ExecutionOptions.ParallelMode() == ParallelMode.None)
        {
            return false;
        }

        if (executionContext.ExecutionOptions.MaxParallelThreads() == 1)
        {
            return false;
        }

        return executionContext.TestCollectionCount > 1;
    }

    private static async Task InitializeSingletonServices(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
    {
        var singletonInstances = serviceCollection
            .Where(x =>
                x.Lifetime == ServiceLifetime.Singleton &&
                typeof(IAccelergreatInitialize).IsAssignableFrom(x.ImplementationType ?? x.ServiceType))
            .Select(x => (IAccelergreatInitialize)serviceProvider.GetRequiredService(x.ServiceType))
            .ToArray();

        foreach (var singletonInstance in singletonInstances)
        {
            await singletonInstance.InitializeAsync();
        }
    }
}
