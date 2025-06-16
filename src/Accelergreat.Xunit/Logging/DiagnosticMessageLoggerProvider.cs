using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Accelergreat.Xunit.Logging;


internal sealed class DiagnosticMessageLoggerProvider : ILoggerProvider
{
    private readonly DiagnosticMessageLogger _logger;

    internal DiagnosticMessageLoggerProvider(IMessageSink diagnosticMessageSink, IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions)
    {
        _logger = new DiagnosticMessageLogger(diagnosticMessageSink, executionMessageSink, executionOptions);
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _logger;
    }

    void IDisposable.Dispose()
    {
    }
}