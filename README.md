# Accelergreat

Accelergreat is a .NET integration testing framework that helps you build fast, reliable tests by managing test environments, component lifecycles, and database reset strategies.

## Who this is for

Use Accelergreat when your tests need real dependencies (database, API host, service-to-service calls), but you still want:

- isolated tests
- predictable setup and teardown
- support for parallel execution

## Package map

- `Accelergreat` - core contracts and environment orchestration
- `Accelergreat.Xunit` - xUnit 2 integration
- `Accelergreat.Xunit3` - xUnit 3 integration
- `Accelergreat.EntityFramework` - shared EF abstractions
- `Accelergreat.EntityFramework.SqlServer` - SQL Server support
- `Accelergreat.EntityFramework.Sqlite` - SQLite support
- `Accelergreat.Web` - ASP.NET Core and Kestrel host integration

## Prerequisites

- .NET SDK 8+ (this repo targets `net8.0`, `net9.0`, `net10.0`)
- Test project using xUnit 2 or xUnit 3
- DB provider package if your tests need relational storage

## Quick start (xUnit 2)

```bash
dotnet add package Accelergreat.Xunit
dotnet add package Accelergreat.EntityFramework.SqlServer
```

Create startup wiring:

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

Create a base test:

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

## Quick start (xUnit 3)

```bash
dotnet add package Accelergreat.Xunit3
dotnet add package Accelergreat.EntityFramework.SqlServer
```

The test shape remains the same (constructor with `IAccelergreatEnvironmentPool`, inherit `AccelergreatXunitTest`).

## Minimal project files

### xUnit 2 test project

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Accelergreat.Xunit" Version="~(version)~" />
    <PackageReference Include="Accelergreat.EntityFramework.SqlServer" Version="~(version)~" />
    <PackageReference Include="xunit" Version="2.*" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.*" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.*" />
  </ItemGroup>
</Project>
```

### xUnit 3 test project

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Accelergreat.Xunit3" Version="~(version)~" />
    <PackageReference Include="Accelergreat.EntityFramework.SqlServer" Version="~(version)~" />
    <PackageReference Include="xunit.v3" Version="3.*" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.*" />
  </ItemGroup>
</Project>
```

## End-to-end example

```csharp
public class AppDatabaseComponent : SqlServerEntityFrameworkDatabaseComponent<AppDbContext>
{
    public AppDatabaseComponent(IConfiguration configuration) : base(configuration)
    {
    }
}

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

public class Startup : IAccelergreatStartup
{
    public void Configure(IAccelergreatBuilder builder)
    {
        builder.AddAccelergreatComponent<AppDatabaseComponent>();
        builder.AddAccelergreatComponent<AppApiComponent>();
    }
}

public class AppApiTests : AccelergreatXunitTest
{
    public AppApiTests(IAccelergreatEnvironmentPool environmentPool) : base(environmentPool)
    {
    }

    [Fact]
    public async Task Should_return_health_ok()
    {
        var client = GetComponent<AppApiComponent>().CreateClient();
        var response = await client.GetAsync("/health");
        response.EnsureSuccessStatusCode();
    }
}
```

## xUnit 2 to xUnit 3 Migration Guide

Use this when moving from `Accelergreat.Xunit` (xUnit 2) to `Accelergreat.Xunit3` (xUnit 3) while keeping behavior as close as possible.

### 1) Replace package reference

```xml
<!-- before -->
<PackageReference Include="Accelergreat.Xunit" Version="~(version)~" />

<!-- after -->
<PackageReference Include="Accelergreat.Xunit3" Version="~(version)~" />
```

### 2) Keep startup and components

No namespace changes are required for your Accelergreat startup/configuration contracts:

- `IAccelergreatStartup`
- `IAccelergreatBuilder`
- `AccelergreatXunitTest`

### 3) Constructor injection expectations

- Preferred constructor signature for Accelergreat tests remains:
  - `IAccelergreatEnvironmentPool environmentPool`
- xUnit3 execution support in this repo is implemented so DI-backed constructor resolution still works for Accelergreat tests.

### 4) Check lifecycle signatures in shared code

When implementing explicit async lifecycle interfaces directly in your own fixtures/tests, verify xUnit3-compatible signatures where required (`ValueTask`-based APIs in xUnit3 surface area). If you only inherit from Accelergreat base types, no manual change is usually needed.

### 5) Update test runner/tooling packages

Make sure your test project uses xUnit3 runner/extensibility packages compatible with `Accelergreat.Xunit3`.

### 6) Verify with a side-by-side run

Recommended migration validation:

1. Keep existing xUnit2 tests/projects.
2. Add mirrored xUnit3 projects from shared sources.
3. Run full suite:

```bash
dotnet test Accelergreat.sln -m:1
```

### Common pitfalls

- Using mixed xUnit2/xUnit3 lifecycle interfaces in one compile target.
- Missing xUnit3 package references in mirrored test projects.
- Assuming fixture constructor behavior without verifying DI fallback path.

## Recommended verification commands

```bash
dotnet restore Accelergreat.sln
dotnet build Accelergreat.sln
dotnet test Accelergreat.sln -m:1
```

## Using AI coding tools with Accelergreat

Accelergreat ships three AI coding agent context files inside the `Accelergreat.Xunit` and `Accelergreat.Xunit3` NuGet packages:

| File | Used by |
|---|---|
| `AGENTS.md` | Claude Code, OpenClaw, OpenAI Codex CLI — full reference for Accelergreat patterns |
| `CLAUDE.md` | Claude Code / OpenClaw — compact quickstart that points to `AGENTS.md` |
| `.cursorrules` | Cursor AI — rules prepended to every request so Cursor writes idiomatic tests |

**How it works:** When you build your project for the first time after adding `Accelergreat.Xunit` or `Accelergreat.Xunit3`, MSBuild automatically copies these files into your solution root (if they don't already exist there). Your AI tool picks them up from there — no extra configuration required.

**Already have these files?** No problem. The copy only runs when the files are absent — it will never overwrite your customised versions.

**Working in this repo?** The files are already here. Open the repo in Cursor or run Claude Code in it and the tools load them automatically.

---

## Troubleshooting

- **Tests cannot resolve `IAccelergreatEnvironmentPool`**
  - verify the project references `Accelergreat.Xunit` or `Accelergreat.Xunit3` (not both)
  - verify framework attribute/targets wiring is present in the test project
- **Behavior differs between xUnit2 and xUnit3 projects**
  - confirm mirrored projects reference shared sources consistently
  - run both suites serially (`-m:1`) to remove concurrency noise first

## FAQ

- **Can I keep xUnit2 and xUnit3 in the same repo?**
  - Yes. Use separate projects and mirror shared test source where appropriate.
- **Do I need to rewrite component/startup code for xUnit3?**
  - Usually no. Most changes are project/package wiring and runner compatibility.
- **Do test constructors still use `IAccelergreatEnvironmentPool`?**
  - Yes, that remains the preferred constructor pattern for Accelergreat tests.

## Package READMEs

- [`src/Accelergreat/README.md`](src/Accelergreat/README.md)
- [`src/Accelergreat.Xunit/README.md`](src/Accelergreat.Xunit/README.md)
- [`src/Accelergreat.Xunit3/README.md`](src/Accelergreat.Xunit3/README.md)
- [`src/Accelergreat.EntityFramework/README.md`](src/Accelergreat.EntityFramework/README.md)
- [`src/Accelergreat.EntityFramework.SqlServer/README.md`](src/Accelergreat.EntityFramework.SqlServer/README.md)
- [`src/Accelergreat.EntityFramework.Sqlite/README.md`](src/Accelergreat.EntityFramework.Sqlite/README.md)
- [`src/Accelergreat.Web/README.md`](src/Accelergreat.Web/README.md)

