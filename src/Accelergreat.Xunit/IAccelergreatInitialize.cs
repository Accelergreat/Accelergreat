namespace Accelergreat.Xunit;

/// <summary>
/// Used to initialize objects managed by Accelergreat dependency injection at the start of their lifetime and before the objects are injected.
/// </summary>
public interface IAccelergreatInitialize
{
    Task InitializeAsync();
}