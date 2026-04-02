# AGENTS.md — Accelergreat Integration Testing Framework

This file is for AI coding agents (Cursor, Copilot, Claude, OpenClaw, etc.). Read this before writing any integration tests in this repository or any project that uses Accelergreat.

---

## What is Accelergreat?

Accelergreat is a .NET integration testing framework built on top of xUnit. It manages test dependencies (databases, APIs, services) automatically, resets state between tests in milliseconds, and runs tests in parallel using an environment pool. **Do not use raw `WebApplicationFactory`, `IClassFixture<DatabaseFixture>`, or manual EF setup/teardown in this codebase.** Use Accelergreat components instead.

---

## NuGet Packages

Install the packages you need:

| Package | Purpose |
|---|---|
| `Accelergreat.Xunit` | Core xUnit integration (always required) |
| `Accelergreat` | Base package for custom components |
| `Accelergreat.EntityFramework.SqlServer` | SQL Server + Entity Framework support |
| `Accelergreat.EntityFramework.Sqlite` | SQLite + Entity Framework support |
| `Accelergreat.Web` | Web API test hosting (`WebAppComponent`, `KestrelWebAppComponent`) |

```bash
dotnet add package Accelergreat.Xunit
dotnet add package Accelergreat.EntityFramework.SqlServer   # or Sqlite
dotnet add package Accelergreat.Web                         # if testing web APIs
```

---

## Core Concepts

### 1. Startup class

Every test project needs exactly one `Startup` class implementing `IAccelergreatStartup`. This registers the components for the test assembly.

```csharp
public class Startup : IAccelergreatStartup
{
    public void Configure(IAccelergreatBuilder builder)
    {
        // Register in dependency order — databases first, then APIs
        builder.AddAccelergreatComponent<MyDatabaseComponent>();
        builder.AddAccelergreatComponent<MyApiComponent>();
    }
}
```

**Component registration methods on `IAccelergreatBuilder`:**
- `AddAccelergreatComponent<T>()` — default (scoped: one instance per xUnit test collection)
- `AddSingletonAccelergreatComponent<T>()` — one instance for the whole assembly
- `AddTransientAccelergreatComponent<T>()` — one instance per test

### 2. Components

A component represents a single test dependency. Accelergreat initializes them once (or per-scope) and resets them between tests. You never manually set up or tear down state.

**SQL Server component:**

```csharp
public class MyDatabaseComponent : SqlServerEntityFrameworkDatabaseComponent<MyDbContext>
{
    public MyDatabaseComponent(IConfiguration configuration) : base(configuration)
    {
    }

    // Optional: seed global data once when the database is first created
    protected override async Task OnDatabaseInitializedAsync(MyDbContext context)
    {
        context.Categories.Add(new Category { Name = "Default" });
        await context.SaveChangesAsync();
    }
}
```

**SQLite component:**

```csharp
public class MyDatabaseComponent : SqliteEntityFrameworkDatabaseComponent<MyDbContext>
{
    public MyDatabaseComponent(IConfiguration configuration) : base(configuration)
    {
    }
}
```

**Web API component (Program.cs / Startup.cs entry point):**

```csharp
// WebAppComponent — uses WebApplicationFactory (in-process, no real port)
public class MyApiComponent : WebAppComponent<Program>
{
    protected override void BuildConfiguration(
        IConfigurationBuilder configurationBuilder,
        IReadOnlyAccelergreatEnvironmentPipelineData environmentData)
    {
        // Wire database connection string from the Accelergreat-managed DB
        configurationBuilder.AddEntityFrameworkDatabaseConnectionString<MyDbContext>(
            "DefaultConnection", environmentData);
    }
}

// KestrelWebAppComponent — binds to a real Kestrel port (use for microservice tests)
public class MyApiComponent : KestrelWebAppComponent<Program>
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder,
        IConfiguration configuration,
        IReadOnlyAccelergreatEnvironmentPipelineData environmentData)
    {
        builder.UseEnvironment("IntegrationTesting");

        builder.ConfigureServices(services =>
        {
            services.AddAccelergreatDbContext<MyDbContext>(environmentData, useTransactionOverriding: true);
        });
    }
}
```

**Custom component (e.g. Redis, message bus):**

```csharp
public class MyRedisComponent : IAccelergreatComponent
{
    private IConnectionMultiplexer _redis = null!;

    public async Task InitializeAsync(IAccelergreatEnvironmentPipelineData environmentData)
    {
        _redis = await ConnectionMultiplexer.ConnectAsync("localhost:6379");
        environmentData.Add("RedisConnection", _redis);
    }

    public async Task ResetAsync()
    {
        await _redis.GetDatabase().FlushDatabaseAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _redis.DisposeAsync();
    }
}
```

### 3. Test classes

Every test class inherits from `AccelergreatXunitTest` and takes `IAccelergreatEnvironmentPool` in its constructor. Use `GetComponent<T>()` to access components.

```csharp
public class OrderTests : AccelergreatXunitTest
{
    public OrderTests(IAccelergreatEnvironmentPool environmentPool) : base(environmentPool)
    {
    }

    [Fact]
    public async Task CreateOrder_PersistsToDatabase()
    {
        // Get components — these are pre-initialized, fresh state guaranteed
        var db = GetComponent<MyDatabaseComponent>();
        var api = GetComponent<MyApiComponent>();

        // Arrange
        await using var ctx = db.DbContextFactory.NewDbContext();
        ctx.Products.Add(new Product { Name = "Widget", Price = 9.99m });
        await ctx.SaveChangesAsync();

        // Act
        var client = api.CreateClient();
        var response = await client.GetAsync("/api/products");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        // No cleanup needed — Accelergreat resets state automatically
    }
}
```

---

## Configuration Files

Every test project needs an `accelergreat.json` file. Use environment-specific overrides (`accelergreat.development.json`, `accelergreat.ci.json`).

**`accelergreat.json` (base — usually empty or with CI defaults):**

```json
{
  "$schema": "https://cdn.accelergreat.net/configuration/0.2.5/schema.json"
}
```

**`accelergreat.development.json` (fast local resets):**

```json
{
  "$schema": "https://cdn.accelergreat.net/configuration/0.2.5/schema.json",
  "SqlServerEntityFramework": {
    "ConnectionString": "Server=localhost;TrustServerCertificate=True",
    "ResetStrategy": "Transactions"
  }
}
```

**`accelergreat.ci.json` (snapshot resets for CI):**

```json
{
  "$schema": "https://cdn.accelergreat.net/configuration/0.2.5/schema.json",
  "SqlServerEntityFramework": {
    "ConnectionString": "Server=ci-server;Database=TestDb;User Id=sa;Password=...;TrustServerCertificate=True",
    "ResetStrategy": "SnapshotRollback"
  }
}
```

**Reset strategies:**
| Strategy | Speed | Use case |
|---|---|---|
| `Transactions` | 0–3ms | Local development (SQL Server savepoints) |
| `SnapshotRollback` | 80–150ms | CI / environments without transaction isolation |

**Select environment:**
```bash
export ACCELERGREAT_ENVIRONMENT=development  # loads accelergreat.development.json
```

---

## Parallel Execution

Accelergreat handles parallelism through its environment pool. Enable xUnit parallelism in `xunit.runner.json`:

```json
{
  "maxParallelThreads": 4,
  "parallelizeTestCollections": true
}
```

Do **not** add test classes to `[Collection]` groups unless you need logical grouping — Accelergreat parallelises automatically.

---

## Microservices

When testing multiple services that call each other, use `KestrelWebAppComponent` so each service gets a real HTTP address:

```csharp
public class PaymentServiceComponent : KestrelWebAppComponent<PaymentService.Program>
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder,
        IConfiguration configuration,
        IReadOnlyAccelergreatEnvironmentPipelineData environmentData)
    {
        // Get the URL of the order service that was initialized before this one
        var orderServiceUrl = environmentData.GetKestrelWebAppHttpBaseAddress<OrderService.Program>();

        builder.ConfigureAppConfiguration(cfg =>
            cfg.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Services:OrderService:BaseUrl"] = orderServiceUrl
            }));
    }
}
```

Register in dependency order in `Startup.cs` — the service whose URL is referenced must be registered first.

---

## Project Structure

A typical Accelergreat test project looks like this:

```
MyProject.Tests/
├── MyProject.Tests.csproj
├── Startup.cs                     ← IAccelergreatStartup
├── accelergreat.json              ← base config (CI defaults)
├── accelergreat.development.json  ← local dev overrides
├── xunit.runner.json              ← parallelism config
├── Components/
│   ├── MyDatabaseComponent.cs
│   └── MyApiComponent.cs
└── Tests/
    └── OrderTests.cs
```

---

## Rules for AI Agents

1. **Always inherit from `AccelergreatXunitTest`** — never from `IClassFixture<T>` directly.
2. **Always create a `Startup` class** implementing `IAccelergreatStartup` in each test project.
3. **Never manually create/destroy databases** — use `SqlServerEntityFrameworkDatabaseComponent` or `SqliteEntityFrameworkDatabaseComponent`.
4. **Never use `WebApplicationFactory<T>` directly** — use `WebAppComponent<T>` or `KestrelWebAppComponent<T>`.
5. **Always use `DbContextFactory.NewDbContext()`** inside tests — do not inject `DbContext` directly.
6. **Component order matters** in `Startup.Configure` — register dependencies before dependents.
7. **No teardown code** — Accelergreat's `ResetAsync` handles state. Remove any `RemoveRange`, `DeleteDatabase`, or manual cleanup.
8. **Add `$schema`** to every `accelergreat.json` for IDE autocomplete.
9. **Do not add `[Collection]` attributes** unless tests must share state — it prevents parallelism.
10. **Pass env vars for config** — use `ACCELERGREAT_ENVIRONMENT` to switch between profiles, never hardcode connection strings in test code.

---

## Documentation & Resources

- 📖 [Full documentation](https://docs.accelergreat.net/)
- 📦 [NuGet packages](https://www.nuget.org/profiles/Nanogunn)
- 🐛 [GitHub Issues](https://github.com/Accelergreat/Accelergreat/issues)
- 💬 [Discord](https://discord.com/channels/1175044305988091995/1175044307032481804)
