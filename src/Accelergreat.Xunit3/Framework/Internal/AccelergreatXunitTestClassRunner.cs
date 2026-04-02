using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.v3;
using System.Reflection;

namespace Accelergreat.Xunit.Framework.Internal;

internal sealed class AccelergreatXunitTestClassRunner : XunitTestClassRunner
{
    private readonly IServiceScope _collectionServiceScope;
    private readonly ILogger _logger;

    internal AccelergreatXunitTestClassRunner(IServiceScope collectionServiceScope, ILogger logger)
    {
        _collectionServiceScope = collectionServiceScope;
        _logger = logger;
    }

    protected override ValueTask<object?> GetConstructorArgument(
        XunitTestClassRunnerContext ctxt,
        ConstructorInfo constructor,
        int index,
        ParameterInfo parameter)
    {
        var baseArgument = base.GetConstructorArgument(ctxt, constructor, index, parameter);

        if (baseArgument.IsCompletedSuccessfully && baseArgument.Result is not null)
        {
            return baseArgument;
        }

        try
        {
            var argumentValue = _collectionServiceScope.ServiceProvider.GetService(parameter.ParameterType);
            return new ValueTask<object?>(argumentValue);
        }
        catch (Exception exception)
        {
            _logger.LogCritical(exception, "{message}", exception.Message);
            throw;
        }
    }
}
