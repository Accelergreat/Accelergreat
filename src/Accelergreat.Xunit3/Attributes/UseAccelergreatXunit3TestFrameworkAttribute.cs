using Accelergreat.Xunit.Framework;
using Xunit.v3;

namespace Accelergreat.Xunit.Attributes;

[AttributeUsage(AttributeTargets.Assembly)]
public sealed class UseAccelergreatXunit3TestFrameworkAttribute : Attribute, ITestFrameworkAttribute
{
    public Type FrameworkType => typeof(AccelergreatXunitTestFramework);
}
