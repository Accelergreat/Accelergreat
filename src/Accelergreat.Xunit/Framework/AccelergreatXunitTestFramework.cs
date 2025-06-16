using System.ComponentModel;
using System.Reflection;
using Accelergreat.Xunit.Framework.Internal;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Accelergreat.Xunit.Framework;


public sealed class AccelergreatXunitTestFramework : XunitTestFramework
{
    public AccelergreatXunitTestFramework(IMessageSink messageSink) : base(messageSink)
    {
    }

    protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
    {
        return new AccelergreatXunitTestFrameworkExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink);
    }
}