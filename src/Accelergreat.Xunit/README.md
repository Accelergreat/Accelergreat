# Accelergreat.Xunit

xUnit 2 integration package for Accelergreat.

## Install

```bash
dotnet add package Accelergreat.Xunit
```

## Minimal project file

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Accelergreat.Xunit" Version="~(version)~" />
    <PackageReference Include="xunit" Version="2.*" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.*" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.*" />
  </ItemGroup>
</Project>
```

## What you get

- `AccelergreatXunitTest` base class for integration tests
- constructor DI support via `IAccelergreatEnvironmentPool`
- startup discovery through `IAccelergreatStartup`
- environment pooling and reset orchestration

## Basic setup

```csharp
public class Startup : IAccelergreatStartup
{
    public void Configure(IAccelergreatBuilder builder)
    {
        builder.AddAccelergreatComponent<AppDatabaseComponent>();
        builder.AddAccelergreatComponent<AppApiComponent>();
    }
}
```

```csharp
public class ApiTests : AccelergreatXunitTest
{
    public ApiTests(IAccelergreatEnvironmentPool environmentPool) : base(environmentPool)
    {
    }

    [Fact]
    public async Task Should_return_ok()
    {
        var client = GetComponent<AppApiComponent>().CreateClient();
        var response = await client.GetAsync("/health");
        response.EnsureSuccessStatusCode();
    }
}
```

## Notes

- Use this package for xUnit 2 test projects.
- For xUnit 3 projects, use `Accelergreat.Xunit3`.

## Validation command

```bash
dotnet test Accelergreat.sln -m:1
```

## FAQ

- **Can I use this with xUnit3?**
  - No. Use `Accelergreat.Xunit3` for xUnit3 projects.
- **Do I need special constructor parameters?**
  - Use `IAccelergreatEnvironmentPool` in your test constructor.
- **Can I run xUnit2 and xUnit3 side by side?**
  - Yes, in separate projects.

## Migrating to xUnit 3

Migration steps and pitfalls are documented in:

- [`../../README.md`](../../README.md#xunit-2-to-xunit-3-migration-guide)
- [`../Accelergreat.Xunit3/README.md`](../Accelergreat.Xunit3/README.md)
