# Accelergreat

Core package containing the contracts and orchestration primitives that other Accelergreat integration packages build on.

## What this package provides

- environment lifecycle contracts
- component registration and activation contracts
- shared pipeline data interfaces used by component packages

## Typical usage

Most test projects consume this indirectly through:

- `Accelergreat.Xunit` or `Accelergreat.Xunit3`
- one or more component packages (`Accelergreat.EntityFramework.*`, `Accelergreat.Web`)

## Direct usage scenarios

Reference `Accelergreat` directly if you are:

- implementing custom components
- integrating Accelergreat into a custom test harness
- sharing core contracts across package boundaries

## Example: custom component contract usage

```csharp
public class CustomComponent : IAccelergreatComponent
{
    public Task InitializeAsync(IAccelergreatEnvironmentPipelineData environmentData)
    {
        return Task.CompletedTask;
    }

    public Task ResetAsync()
    {
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
```

## FAQ

- **Should I install this package directly in test projects?**
  - Usually you install `Accelergreat.Xunit` or `Accelergreat.Xunit3` plus component packages; this package comes transitively.
- **When do I use this package directly?**
  - When building custom integrations/components against Accelergreat core contracts.

## Related docs

- [`../../README.md`](../../README.md)
- [`../Accelergreat.Xunit/README.md`](../Accelergreat.Xunit/README.md)
- [`../Accelergreat.Xunit3/README.md`](../Accelergreat.Xunit3/README.md)
