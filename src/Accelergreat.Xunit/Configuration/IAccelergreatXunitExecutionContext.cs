using System.ComponentModel;
using Xunit.Abstractions;

namespace Accelergreat.Xunit.Configuration;


public interface IAccelergreatXunitExecutionContext
{
    ITestFrameworkExecutionOptions ExecutionOptions { get; }
    int TestCollectionCount { get; }
}