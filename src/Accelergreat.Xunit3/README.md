# Accelergreat.Xunit3

xUnit 3 integration package for Accelergreat.

This package keeps the same day-to-day test shape used by `Accelergreat.Xunit`, while adapting internals to xUnit 3 extensibility APIs.

## Install

```bash
dotnet add package Accelergreat.Xunit3
```

## Minimal project file

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Accelergreat.Xunit3" Version="~(version)~" />
    <PackageReference Include="xunit.v3.mtp-off" Version="4.*" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="18.*" />
  </ItemGroup>
</Project>
```

xUnit v3 4.0 defaults to Microsoft Testing Platform. Accelergreat.Xunit3 uses a custom VSTest test framework, so test projects should reference `xunit.v3.mtp-off` (not `xunit.v3`) to keep `dotnet test` on VSTest.

## What you get

- `AccelergreatXunitTest` base class
- constructor DI support for `IAccelergreatEnvironmentPool`
- startup discovery through `IAccelergreatStartup`
- environment pooling and reset behavior aligned with xUnit2 package goals

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

## xUnit2 to xUnit3 migration checklist

1. Replace `Accelergreat.Xunit` with `Accelergreat.Xunit3`.
2. Keep `IAccelergreatStartup` and component registration as-is.
3. Keep constructor injection via `IAccelergreatEnvironmentPool` for Accelergreat tests.
4. Verify xUnit3-compatible runner/extensibility package references in the test project.
5. Run full suite with mirrored projects/sources to validate parity.

## Validation command

```bash
dotnet test Accelergreat.sln -m:1
```

## Common pitfalls

- Compiling xUnit2 and xUnit3 lifecycle signatures in the same target.
- Missing xUnit3 runner dependencies in test projects.
- Assuming fixture wiring without validating the custom framework activation path.

## FAQ

- **Do I need to change all test constructors to fixtures?**
  - No. Keep `IAccelergreatEnvironmentPool` where your tests inherit `AccelergreatXunitTest`.
- **Can I migrate gradually?**
  - Yes. Keep xUnit2 projects and introduce mirrored xUnit3 projects.
- **What is the first migration validation step?**
  - Run serial full-suite tests and compare outcomes between xUnit2 and xUnit3 projects.

## When to use `Accelergreat.Xunit` instead

Use `Accelergreat.Xunit` if your test project is still on xUnit 2 and you are not yet migrating.

## Full migration guide

See [`../../README.md`](../../README.md#xunit-2-to-xunit-3-migration-guide).
