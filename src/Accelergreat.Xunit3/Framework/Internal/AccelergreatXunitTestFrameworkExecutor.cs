using Accelergreat.Configuration;
using Accelergreat.Xunit.Configuration;
using Accelergreat.Xunit.Logging;
using Microsoft.Extensions.Logging;
using Xunit.Sdk;
using Xunit.v3;

namespace Accelergreat.Xunit.Framework.Internal;

internal sealed class AccelergreatXunitTestFrameworkExecutor : XunitTestFrameworkExecutor
{
    internal AccelergreatXunitTestFrameworkExecutor(IXunitTestAssembly testAssembly)
        : base(testAssembly)
    {
    }

    public override async ValueTask RunTestCases(
        IReadOnlyCollection<IXunitTestCase> testCases,
        IMessageSink executionMessageSink,
        ITestFrameworkExecutionOptions executionOptions,
        CancellationToken cancellationToken)
    {
        ILogger? logger = null;

        try
        {
            var loggerProvider = new DiagnosticMessageLoggerProvider(executionMessageSink, executionMessageSink, executionOptions);
            logger = loggerProvider.CreateLogger(string.Empty);

            var configuration = await AccelergreatConfigurationProvider.GetAccelergreatConfigurationAsync();
            var executionContext = AccelergreatXunitExecutionContext.Build(executionOptions, testCases);
            var startup = GetStartup();

            var runner = new AccelergreatXunitTestAssemblyRunner(loggerProvider, configuration, executionContext, startup);
            await runner.Run(TestAssembly, testCases, executionMessageSink, executionOptions, cancellationToken);
        }
        catch (Exception exception)
        {
            logger?.LogCritical(exception, "{message}", exception.Message);
            throw;
        }
    }

    private static IAccelergreatStartup? GetStartup()
    {
        var startupConstructors = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(x => !x.IsDynamic)
            .SelectMany(x => x.GetTypes())
            .Where(x => typeof(IAccelergreatStartup).IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false })
            .Select(x => x.GetConstructor(Type.EmptyTypes))
            .Where(x => x is not null)
            .ToArray();

        if (startupConstructors.Length > 1)
        {
            throw new InvalidOperationException("Only one IAccelergreatStartup implementation is supported per test assembly.");
        }

        return startupConstructors.Length == 1
            ? (IAccelergreatStartup)startupConstructors[0]!.Invoke(Array.Empty<object>())
            : null;
    }
}
