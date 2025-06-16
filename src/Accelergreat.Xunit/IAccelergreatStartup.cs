namespace Accelergreat.Xunit;

/// <summary>
/// Accelergreat startup.
/// </summary>
public interface IAccelergreatStartup
{
    /// <summary>
    /// Add services to the <see cref="IAccelergreatBuilder"/> that is used by Accelergreat to inject services into test class constructors.
    /// </summary>
    /// <remarks>
    /// The following service lifetimes apply to test suite execution:
    /// <para><b>Singleton:</b> 1 instance per test assembly.</para>
    /// <para><b>Scoped:</b> 1 instance per test <see href="https://xunit.net/docs/running-tests-in-parallel#parallelism-in-test-frameworks">collection</see>.</para>
    /// <para><b>Transient:</b> 1 instance per test.</para>
    /// </remarks>
    public void Configure(IAccelergreatBuilder builder);
}