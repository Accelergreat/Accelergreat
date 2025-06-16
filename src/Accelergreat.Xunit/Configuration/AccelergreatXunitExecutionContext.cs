using System.ComponentModel;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Accelergreat.Xunit.Configuration;


internal sealed class AccelergreatXunitExecutionContext : IAccelergreatXunitExecutionContext
{
    private AccelergreatXunitExecutionContext(ITestFrameworkExecutionOptions executionOptions, int testCollectionCount)
    {
        ExecutionOptions = executionOptions;
        TestCollectionCount = testCollectionCount;
    }

    public ITestFrameworkExecutionOptions ExecutionOptions { get; }

    public int TestCollectionCount { get; }

    public static AccelergreatXunitExecutionContext Build(
        ITestFrameworkExecutionOptions executionOptions,
        IEnumerable<IXunitTestCase> testCases)
    {
        var testCollectionCount = testCases
            .GroupBy(x => x.TestMethod.TestClass.TestCollection, TestCollectionComparer.Instance)
            .Count();

        return new AccelergreatXunitExecutionContext(executionOptions, testCollectionCount);
    }
}