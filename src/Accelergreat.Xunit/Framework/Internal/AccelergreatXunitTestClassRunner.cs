using System.ComponentModel;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Accelergreat.Xunit.Framework.Internal;


internal sealed class AccelergreatXunitTestClassRunner : XunitTestClassRunner
{
    private readonly IServiceScope _collectionServiceScope;
    private readonly ILogger _logger;

    internal AccelergreatXunitTestClassRunner(
        IServiceScope collectionServiceScope,
        ILogger logger,
        ITestClass testClass, 
        IReflectionTypeInfo @class, 
        IEnumerable<IXunitTestCase> testCases,
        IMessageSink diagnosticMessageSink, 
        IMessageBus messageBus,
        ITestCaseOrderer testCaseOrderer, 
        ExceptionAggregator aggregator, 
        CancellationTokenSource cancellationTokenSource, 
        IDictionary<Type, object> collectionFixtureMappings) 
        : base(
            testClass,
            @class, 
            testCases, 
            diagnosticMessageSink, 
            messageBus, 
            testCaseOrderer, 
            aggregator, 
            cancellationTokenSource, 
            collectionFixtureMappings)
    {
        _collectionServiceScope = collectionServiceScope;
        _logger = logger;
    }

    protected override bool TryGetConstructorArgument(
        ConstructorInfo constructor, 
        int index, 
        ParameterInfo parameter,
        out object argumentValue)
    {
        bool result;

        if (!(result = base.TryGetConstructorArgument(constructor, index, parameter, out argumentValue)))
        {
            try
            {
                if ((argumentValue = _collectionServiceScope.ServiceProvider.GetService(parameter.ParameterType)!) is not null)
                {
                    result = true;
                }
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, "{message}", exception.Message);
                throw;
            }
        }

        return result;
    }
}