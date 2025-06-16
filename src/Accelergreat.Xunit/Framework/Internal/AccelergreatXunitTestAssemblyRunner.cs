using System.ComponentModel;
using System.Diagnostics;
using Accelergreat.Environments;
using Accelergreat.Environments.Pooling;
using Accelergreat.Xunit.Configuration;
using Accelergreat.Xunit.Environments.Pooling;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Accelergreat.Xunit.Framework.Internal;


internal sealed class AccelergreatXunitTestAssemblyRunner : XunitTestAssemblyRunner
{
    private const string XunitExecutionDisableParallelizationSettingName = "xunit.execution.DisableParallelization";

    private readonly ILogger _logger;
    private readonly IServiceCollection _serviceCollection;
    private readonly ServiceProvider _serviceProvider;
    private readonly Stopwatch _assemblyTestStopwatch;

    internal AccelergreatXunitTestAssemblyRunner(
        ILoggerProvider loggerProvider,
        IConfiguration configuration,
        IAccelergreatXunitExecutionContext executionContext,
        IAccelergreatStartup? startup,
        ITestAssembly testAssembly,
        IEnumerable<IXunitTestCase> testCases,
        IMessageSink diagnosticMessageSink,
        IMessageSink executionMessageSink,
        ITestFrameworkExecutionOptions executionOptions)
        : base(testAssembly, testCases, diagnosticMessageSink, executionMessageSink, executionOptions)
    {
        _logger = loggerProvider.CreateLogger(string.Empty);

        _logger.LogInformation("Thank you for using Accelergreat. Your feedback is welcome.");

        _serviceCollection = new ServiceCollection();

        startup?.Configure(new AccelergreatBuilder(_serviceCollection, configuration));

        AddDefaultServices(configuration, loggerProvider, executionContext);

        _serviceProvider = _serviceCollection.BuildServiceProvider();

        _assemblyTestStopwatch = new Stopwatch();
    }

    protected override string GetTestFrameworkDisplayName()
    {
        return "Accelergreat";
    }

    protected override async Task AfterTestAssemblyStartingAsync()
    {
        await Aggregator.RunAsync(async () =>
        {
            try
            {
                await InitializeSingletonServices(_serviceCollection, _serviceProvider);
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, "{message}", exception.Message);
                throw;
            }
        });

        await base.AfterTestAssemblyStartingAsync();

        _assemblyTestStopwatch.Start();
    }

    protected override async Task BeforeTestAssemblyFinishedAsync()
    {
        _assemblyTestStopwatch.Stop();

        _logger.LogInformation("Finished executing tests in {elapsedMilliseconds}ms", _assemblyTestStopwatch.ElapsedMilliseconds);

        await Aggregator.RunAsync(async () =>
        {
            try
            {
                await _serviceProvider.DisposeAsync();
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, "{message}", exception.Message);
                throw;
            }
        });

        await base.BeforeTestAssemblyFinishedAsync();
    }

    protected override Task<RunSummary> RunTestCollectionAsync(IMessageBus messageBus, ITestCollection testCollection, IEnumerable<IXunitTestCase> testCases,
        CancellationTokenSource cancellationTokenSource)
    {
        return new AccelergreatXunitTestAssemblyTestCollectionRunner(
                _serviceCollection,
                _serviceProvider,
                _logger,
                testCollection,
                testCases,
                DiagnosticMessageSink,
                messageBus,
                TestCaseOrderer,
                new ExceptionAggregator(Aggregator),
                cancellationTokenSource)
            .RunAsync();
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

        if (CanExecuteParallel(executionContext, out var executeSequentialReason))
        {
            _logger.LogInformation("Test execution strategy set to parallel.");
            _serviceCollection.AddSingleton<IAccelergreatEnvironmentPool, ParallelAccelergreatXunitEnvironmentPool>();
        }
        else
        {
            _logger.LogInformation("Test execution strategy set to sequential. Reason: {executeSequentialReason}", executeSequentialReason);
            ExecutionOptions.SetValue(XunitExecutionDisableParallelizationSettingName, true);
            _serviceCollection.AddSingleton<IAccelergreatEnvironmentPool, SingletonAccelergreatXunitEnvironmentPool>();
        }
    }

    private static bool CanExecuteParallel(
        IAccelergreatXunitExecutionContext executionContext,
        out string executeSequentialReason)
    {
        if (executionContext.ExecutionOptions.DisableParallelization() ?? false)
        {
            executeSequentialReason = "xunit configuration (parallelization disabled).";

            return false;
        }

        if (executionContext.ExecutionOptions.MaxParallelThreads() == 1)
        {
            executeSequentialReason = "xunit configuration (max threads set to 1).";

            return false;
        }

        if (executionContext.TestCollectionCount == 1)
        {
            executeSequentialReason = "only 1 test collection queued for execution.";

            return false;
        }

        executeSequentialReason = string.Empty;
        return true;
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