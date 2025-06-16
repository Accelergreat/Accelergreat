using System.ComponentModel;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Accelergreat.Xunit.Framework;


public sealed class AccelergreatTestFrameworkTypeDiscoverer : ITestFrameworkTypeDiscoverer
{
    Type ITestFrameworkTypeDiscoverer.GetTestFrameworkType(IAttributeInfo attribute)
    {
        return typeof(AccelergreatXunitTestFramework);
    }
}