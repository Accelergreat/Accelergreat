using Xunit.Sdk;
using Xunit.v3;

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
        IReadOnlyCollection<IXunitTestCase> testCases)
    {
        var testCollectionCount = testCases
            .GroupBy(x => x.TestCollection, TestCollectionComparer<IXunitTestCollection>.Instance)
            .Count();

        return new AccelergreatXunitExecutionContext(executionOptions, testCollectionCount);
    }
}
