using Xunit.Sdk;

namespace Accelergreat.Xunit.Configuration;

public interface IAccelergreatXunitExecutionContext
{
    ITestFrameworkExecutionOptions ExecutionOptions { get; }
    int TestCollectionCount { get; }
}
