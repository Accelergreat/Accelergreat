using Accelergreat.Xunit.Framework.Internal;
using Xunit.v3;
using System.Reflection;

namespace Accelergreat.Xunit.Framework;

public sealed class AccelergreatXunitTestFramework : XunitTestFramework
{
    protected override ITestFrameworkExecutor CreateExecutor(Assembly assembly)
    {
        return new AccelergreatXunitTestFrameworkExecutor(new XunitTestAssembly(assembly, configFilePath: null));
    }
}
