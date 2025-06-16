using Xunit.Sdk;

namespace Accelergreat.Xunit.Attributes;

/// <summary>
/// Configures the test assembly to use the Accelergreat extended Xunit test framework.
/// <para>Use in conjunction with <see cref="IAccelergreatStartup"/>.</para>
/// </summary>
[TestFrameworkDiscoverer("Accelergreat.Xunit.Framework.AccelergreatTestFrameworkTypeDiscoverer", "Accelergreat.Xunit")]
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class UseAccelergreatXunitTestFrameworkAttribute : Attribute, ITestFrameworkAttribute
{
}