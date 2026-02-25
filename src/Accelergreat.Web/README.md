# Accelergreat.Web

Web and ASP.NET Core integration helpers for Accelergreat tests.

## What this package provides

- Web app component support for in-process and Kestrel-hosted scenarios.
- Configuration helpers for wiring test environment data into web hosts.

## Typical usage

Use this package with `Accelergreat.Xunit` or `Accelergreat.Xunit3` and a database component package when needed.

## Example component

```csharp
public class AppApiComponent : WebAppComponent<Program>
{
    protected override void BuildConfiguration(
        IConfigurationBuilder configurationBuilder,
        IReadOnlyAccelergreatEnvironmentPipelineData environmentData)
    {
        configurationBuilder.AddEntityFrameworkDatabaseConnectionString<AppDbContext>(
            "ConnectionStrings:DefaultConnection", environmentData);
    }
}
```

## Common scenarios

- testing ASP.NET Core APIs end-to-end
- injecting test-time connection strings and service endpoints
- composing multi-service integration tests with Kestrel-hosted components

## Troubleshooting

- **API starts but DB connection fails**
  - verify the config key used in `AddEntityFrameworkDatabaseConnectionString`
  - ensure the expected DB component is registered before API component
- **Cross-service calls fail**
  - verify base-address wiring from environment pipeline data

## FAQ

- **Should I use in-process or Kestrel-hosted components?**
  - Use the component type that best matches your integration scope; Kestrel-hosted is common for microservice-style scenarios.
- **How do I pass DB connection strings into the API under test?**
  - Use `AddEntityFrameworkDatabaseConnectionString` in `BuildConfiguration`.

## Related docs

- [`../../README.md`](../../README.md)
- [`../Accelergreat.Xunit/README.md`](../Accelergreat.Xunit/README.md)
- [`../Accelergreat.Xunit3/README.md`](../Accelergreat.Xunit3/README.md)
