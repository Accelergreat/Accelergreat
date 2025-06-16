using System.ComponentModel;
using System.Reflection;
using Accelergreat.Configuration;
using Accelergreat.Xunit.Configuration;
using Accelergreat.Xunit.Logging;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Accelergreat.Xunit.Framework.Internal;


internal sealed class AccelergreatXunitTestFrameworkExecutor : XunitTestFrameworkExecutor
{
    internal AccelergreatXunitTestFrameworkExecutor(
        AssemblyName assemblyName, 
        ISourceInformationProvider sourceInformationProvider, 
        IMessageSink diagnosticMessageSink) 
        : base(assemblyName, sourceInformationProvider, diagnosticMessageSink)
    {
    }

    protected override async void RunTestCases(
        IEnumerable<IXunitTestCase> testCases, 
        IMessageSink executionMessageSink,
        ITestFrameworkExecutionOptions executionOptions)
    {
        ILogger? logger = null;
        AccelergreatXunitTestAssemblyRunner? assemblyRunner = null;

        try
        {
            var loggerProvider =
                new DiagnosticMessageLoggerProvider(DiagnosticMessageSink, executionMessageSink, executionOptions);

            logger = loggerProvider.CreateLogger(string.Empty);

            var configuration = await AccelergreatConfigurationProvider.GetAccelergreatConfigurationAsync();

            // ReSharper disable once PossibleMultipleEnumeration
            var executionContext = AccelergreatXunitExecutionContext.Build(executionOptions, testCases);

            var startup = GetStartup();

            assemblyRunner = new AccelergreatXunitTestAssemblyRunner(
                // ReSharper disable once PossibleMultipleEnumeration
                loggerProvider, configuration, executionContext, startup, TestAssembly, testCases, DiagnosticMessageSink, executionMessageSink, executionOptions);
        }
        catch (Exception exception)
        {
            logger?.LogCritical(exception, "{message}", exception.Message); 
            assemblyRunner?.Dispose();
            throw;
        }

        using (assemblyRunner)
        {
            await assemblyRunner.RunAsync();
        }
    }

    private IAccelergreatStartup? GetStartup()
    {
        var assembly = ((IReflectionAssemblyInfo)TestAssembly.Assembly).Assembly;

        var startupConstructor = assembly.GetTypes()
            .Where(x => typeof(IAccelergreatStartup).IsAssignableFrom(x))
            .Select(x => x.GetConstructor(Type.EmptyTypes))
            .SingleOrDefault();

        // ReSharper disable once MergeConditionalExpression
        return startupConstructor is not null
            ? (IAccelergreatStartup)startupConstructor.Invoke(Array.Empty<object>())
            : null;
    }
}