using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.v3;

namespace Accelergreat.Xunit.Framework.Internal;

internal sealed class AccelergreatXunitTestCollectionRunner : XunitTestCollectionRunner
{
    private readonly IServiceCollection _serviceCollection;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;
    private IServiceScope? _serviceScope;

    internal AccelergreatXunitTestCollectionRunner(
        IServiceCollection serviceCollection,
        IServiceProvider serviceProvider,
        ILogger logger)
    {
        _serviceCollection = serviceCollection;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async ValueTask<bool> OnTestCollectionStarting(XunitTestCollectionRunnerContext ctxt)
    {
        await ctxt.Aggregator.RunAsync(async () =>
        {
            _serviceScope = _serviceProvider.CreateScope();
            await InitializeScopedServices(_serviceCollection, _serviceScope);
        });

        return await base.OnTestCollectionStarting(ctxt);
    }

    protected override async ValueTask<bool> OnTestCollectionFinished(XunitTestCollectionRunnerContext ctxt, RunSummary summary)
    {
        ctxt.Aggregator.Run(() =>
        {
            _serviceScope?.Dispose();
            _serviceScope = null;
        });

        return await base.OnTestCollectionFinished(ctxt, summary);
    }

    protected override ValueTask<RunSummary> RunTestClass(
        XunitTestCollectionRunnerContext ctxt,
        IXunitTestClass? testClass,
        IReadOnlyCollection<IXunitTestCase> testCases)
    {
        var classRunner = new AccelergreatXunitTestClassRunner(_serviceScope!, _logger);

        return classRunner.Run(
            testClass!,
            testCases,
            ctxt.ExplicitOption,
            ctxt.MessageBus,
            ctxt.Aggregator.Clone(),
            ctxt.CancellationTokenSource,
            ctxt.ParallelMode,
            ctxt.Scheduler,
            ctxt.CollectionFixtureMappings);
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
