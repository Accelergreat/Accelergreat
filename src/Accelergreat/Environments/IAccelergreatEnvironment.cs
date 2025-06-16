using Accelergreat.Components;

namespace Accelergreat.Environments;

/// <summary>
/// An Accelergreat environment holds a collection of <see cref="IAccelergreatComponent"/> and is responsible for managing each component's state.
/// </summary>
public interface IAccelergreatEnvironment : IAsyncDisposable
{
    internal Task InitializeAsync();

    internal Task ResetAsync();

    /// <summary>
    /// Retrieve a <typeparamref name="TAccelergreatComponent"/> instance.
    /// </summary>
    /// <typeparam name="TAccelergreatComponent">The type of the instance to retrieve</typeparam>
    internal TAccelergreatComponent GetComponent<TAccelergreatComponent>()
        where TAccelergreatComponent : IAccelergreatComponent;

    internal TInterface GetComponentByInterface<TInterface>() 
        where TInterface : notnull;

    internal int EnvironmentId { get; }

    internal int AllocationCount { get; }

    internal void IncrementAllocationCount();
}