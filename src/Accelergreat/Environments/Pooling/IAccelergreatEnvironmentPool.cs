namespace Accelergreat.Environments.Pooling;

/// <summary>
/// An Accelergreat environment pool is responsible for the distribution of <see name="IAccelergreatEnvironment"/> and managing each environment's state.
/// </summary>
public interface IAccelergreatEnvironmentPool
{
    /// <summary>
    /// Retrieve a <see name="IAccelergreatEnvironment"/> instance from the pool.
    /// <para>The environment should be back to it's initial state when returned from this method.</para>
    /// </summary>
    internal Task<IAccelergreatEnvironment> RentAsync();

    /// <summary>
    /// Return a <see name="IAccelergreatEnvironment"/> instance to the pool.
    /// <para>The caller's reference to the <see name="IAccelergreatEnvironment"/> instance should be discarded after this method has executed.</para>
    /// </summary>
    internal Task ReturnAsync(IAccelergreatEnvironment environment);
}