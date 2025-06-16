using Accelergreat.Environments;

namespace Accelergreat.Components;

/// <summary>
/// A Accelergreat component is the basis on which allows Accelergreat to function, it represents a single integration test dependency. For example, a database or a web API.
/// </summary>
public interface IAccelergreatComponent : IAsyncDisposable
{
    /// <summary>
    /// Called when initializing a component. The component should be ready to use after this method has executed.
    /// </summary>
    /// <param name="accelergreatEnvironmentPipelineData">
    /// <para>Mutable dictionary that is used to share objects between components.</para>
    /// <para>See <see cref="AccelergreatEnvironmentPipelineDataKeys"/> for pre-defined keys.</para>
    /// </param>
    Task InitializeAsync(IAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData);

    /// <summary>
    /// Called when resetting a component. The component should be back to it's initial state after this method has executed.
    /// </summary>
    Task ResetAsync();
}