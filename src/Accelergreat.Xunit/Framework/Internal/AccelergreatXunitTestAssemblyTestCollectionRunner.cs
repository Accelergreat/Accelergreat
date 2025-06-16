using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Accelergreat.Xunit.Framework.Internal;


internal sealed class AccelergreatXunitTestAssemblyTestCollectionRunner : XunitTestCollectionRunner
{
    private readonly IServiceCollection _serviceCollection;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;
    private IServiceScope? _serviceScope;
            
    internal AccelergreatXunitTestAssemblyTestCollectionRunner(
        IServiceCollection serviceCollection,
        IServiceProvider serviceProvider,
        ILogger logger,
        ITestCollection testCollection, 
        IEnumerable<IXunitTestCase> testCases, 
        IMessageSink diagnosticMessageSink, 
        IMessageBus messageBus, 
        ITestCaseOrderer testCaseOrderer, 
        ExceptionAggregator aggregator, 
        CancellationTokenSource cancellationTokenSource) 
        : base(testCollection, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource)
    {
        _serviceCollection = serviceCollection;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task AfterTestCollectionStartingAsync()
    {
        await Aggregator.RunAsync(async () =>
        {
            try
            {
                _serviceScope = _serviceProvider.CreateScope();

                await InitializeScopedServices(_serviceCollection, _serviceScope);
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, "{message}", exception.Message);
                throw;
            }
        });

        await base.AfterTestCollectionStartingAsync();
    }

    protected override Task BeforeTestCollectionFinishedAsync()
    {
        Aggregator.Run(() =>
        {
            try
            {
                _serviceScope?.Dispose();
                _serviceScope = null;
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, "{message}", exception.Message);
                throw;
            }
        });

        return base.BeforeTestCollectionFinishedAsync();
    }

    protected override Task<RunSummary> RunTestClassAsync(ITestClass testClass, IReflectionTypeInfo @class, IEnumerable<IXunitTestCase> testCases)
    {
        var testClassRunner = new AccelergreatXunitTestClassRunner(
            _serviceScope!,
            _logger,
            testClass,
            @class,
            testCases,
            DiagnosticMessageSink,
            MessageBus,
            TestCaseOrderer,
            new ExceptionAggregator(Aggregator),
            CancellationTokenSource,
            CollectionFixtureMappings);

        return testClassRunner.RunAsync();
    }

    private static Task InitializeScopedServices(IServiceCollection serviceCollection, IServiceScope serviceScope)
    {
        var scopedInstances = serviceCollection
            .Where(x =>
                x.Lifetime == ServiceLifetime.Scoped &&
                typeof(IAccelergreatInitialize).IsAssignableFrom(x.ImplementationType ?? x.ServiceType))
            .Select(x => (IAccelergreatInitialize)serviceScope.ServiceProvider.GetRequiredService(x.ServiceType));

        var initializeScopedTasks = scopedInstances.Select(x => x.InitializeAsync()).ToArray();

        return Task.WhenAll(initializeScopedTasks);
    }
}