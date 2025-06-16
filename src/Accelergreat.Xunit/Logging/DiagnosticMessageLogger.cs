using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Accelergreat.Xunit.Logging;


internal sealed class DiagnosticMessageLogger : ILogger
{
    private readonly IMessageSink _diagnosticMessageSink;
    private readonly IMessageSink _executionMessageSink;
    private readonly ITestFrameworkExecutionOptions _executionOptions;

    internal DiagnosticMessageLogger(IMessageSink diagnosticMessageSink, IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions)
    {
        _diagnosticMessageSink = diagnosticMessageSink;
        _executionMessageSink = executionMessageSink;
        _executionOptions = executionOptions;
    }

    private bool IsEnabled(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Critical => true,
            LogLevel.Error => true,
            _ => _executionOptions.DiagnosticMessages() ?? false
        };
    }

    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        if (exception is not null)
        {
            _executionMessageSink.OnMessage(new ErrorMessage(Array.Empty<ITestCase>(), exception));
        }
        else
        {
            _diagnosticMessageSink.OnMessage(new DiagnosticMessage($"Accelergreat: {formatter(state, exception)}"));
        }
    }

    bool ILogger.IsEnabled(LogLevel logLevel)
    {
        return IsEnabled(logLevel);
    }

    IDisposable ILogger.BeginScope<TState>(TState state)
    {
        return default!;
    }
}