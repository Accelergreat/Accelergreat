# 🧩 Components Guide

Components are the building blocks of Accelergreat. They represent your test dependencies (databases, APIs, services) and handle all the heavy lifting automatically.

> **New to Accelergreat?** Read the [main documentation](index.md) first.

## 🚀 Quick Overview

```csharp
// Components represent your test dependencies
public class OrderDatabaseComponent : SqlServerEntityFrameworkDatabaseComponent<OrderDbContext> { }
public class OrderApiComponent : WebAppComponent<OrderApi.Program> { }
public class PaymentServiceComponent : KestrelWebAppComponent<PaymentService.Program> { }
```

**What components do for you:**
- ✅ **Auto-initialization** - Set up before tests run
- ✅ **Lightning-fast resets** - Clean state between tests (0-3ms!)
- ✅ **Parallel execution** - Scale across multiple environments
- ✅ **Auto-cleanup** - Dispose resources after tests complete

---

## 🔧 Component Registration

### Basic Registration
```csharp
public class Startup : IAccelergreatStartup
{
    public void Configure(IAccelergreatBuilder builder)
    {
        // Order matters - dependencies first!
        builder.AddAccelergreatComponent<DatabaseComponent>();
        builder.AddAccelergreatComponent<ApiComponent>();
        builder.AddAccelergreatComponent<ExternalServiceComponent>();
    }
}
```

### Component Lifetimes

#### Transient (Default) - Per Environment
```csharp
builder.AddAccelergreatComponent<DatabaseComponent>();
// OR
builder.AddTransientAccelergreatComponent<DatabaseComponent>();
```
- **1 instance per environment** (parallel execution)
- **ResetAsync called** between each test
- **Perfect for** databases, APIs that need isolation

#### Singleton - Per Assembly
```csharp
builder.AddSingletonAccelergreatComponent<ExternalServiceComponent>();
```
- **1 instance shared** across all environments
- **ResetAsync NOT called** (shared state)
- **Perfect for** expensive resources, external services

### 💡 Pro Tips

```csharp
// ✅ Good: Database first, then API that depends on it
builder.AddAccelergreatComponent<DatabaseComponent>();
builder.AddAccelergreatComponent<ApiComponent>();

// ❌ Bad: API before database it depends on
builder.AddAccelergreatComponent<ApiComponent>();
builder.AddAccelergreatComponent<DatabaseComponent>();
```

---

## 🗄️ Database Components

### SQL Server - Lightning Fast
```csharp
public class ProductDatabaseComponent : SqlServerEntityFrameworkDatabaseComponent<ProductDbContext>
{
    public ProductDatabaseComponent(IConfiguration configuration) : base(configuration)
    {
    }

    // Add global test data (persists across all tests)
    protected override async Task OnDatabaseInitializedAsync(ProductDbContext context)
    {
        context.Categories.AddRange(
            new Category { Name = "Electronics", IsActive = true },
            new Category { Name = "Books", IsActive = true }
        );
        await context.SaveChangesAsync();
    }

    // Called after each test reset (optional)
    protected override async Task OnDatabaseResetAsync(ProductDbContext context)
    {
        // Re-seed data that doesn't survive resets
        await RefreshCache(context);
    }
}
```

#### Configuration Options
```json
// accelergreat.development.json
{
  "SqlServerEntityFramework": {
    "ResetStrategy": "Transactions",        // 0-3ms resets!
    "CreateStrategy": "Migrations",         // Use EF migrations
    "ConnectionString": "Server=localhost;Database=TestDb;Integrated Security=true;"
  }
}

// accelergreat.ci.json
{
  "SqlServerEntityFramework": {
    "ResetStrategy": "SnapshotRollback",    // 80-150ms resets
    "CreateStrategy": "TypeConfigurations", // Use EF model
    "ConnectionString": "Server=ci-server;Database=TestDb;User Id=testuser;Password=testpass;"
  }
}
```

#### Reset Strategies Comparison
| Strategy | Speed | Reliability | Best For |
|----------|-------|-------------|----------|
| **Transactions** ⚡ | 0-3ms | High | Development, fast feedback |
| **SnapshotRollback** 🔄 | 80-150ms | Very High | CI/CD, production-like |

### SQLite - Ultra Fast
```csharp
public class FastDatabaseComponent : SqliteEntityFrameworkDatabaseComponent<FastDbContext>
{
    public FastDatabaseComponent(IConfiguration configuration) : base(configuration)
    {
    }
}
```

> **📋 Note**: PostgreSQL and In-Memory providers are planned for future releases. Currently supported: SQL Server and SQLite.

---

## 🌐 Web Application Components

### Modern .NET APIs
```csharp
public class OrderApiComponent : WebAppComponent<Program>
{
    protected override void BuildConfiguration(
        IConfigurationBuilder configurationBuilder,
        IReadOnlyAccelergreatEnvironmentPipelineData environmentData)
    {
        // Auto-inject database connection
        configurationBuilder.AddEntityFrameworkDatabaseConnectionString<OrderDbContext>(
            "DefaultConnection", environmentData);
            
        // Add custom configuration
        configurationBuilder.AddInMemoryCollection(new[]
        {
            new KeyValuePair<string, string>("ApiKey", "test-key-123"),
            new KeyValuePair<string, string>("Environment", "Testing")
        });
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder, 
        IConfiguration configuration,
        IReadOnlyAccelergreatEnvironmentPipelineData environmentData)
    {
        builder.UseEnvironment("Testing");
        
        // Configure services for testing
        builder.ConfigureServices(services =>
        {
            services.AddAccelergreatDbContext<OrderDbContext>(
                environmentData, 
                useTransactionOverriding: true  // Handle nested transactions
            );
        });
    }
}
```

### Legacy Startup.cs APIs
```csharp
public class LegacyApiComponent : WebAppComponent<LegacyApi.Startup>
{
    protected override void BuildConfiguration(
        IConfigurationBuilder configurationBuilder,
        IReadOnlyAccelergreatEnvironmentPipelineData environmentData)
    {
        configurationBuilder.AddEntityFrameworkDatabaseConnectionString<LegacyDbContext>(
            "DefaultConnection", environmentData);
    }
}
```

### Kestrel Components (Microservices)
```csharp
public class PaymentServiceComponent : KestrelWebAppComponent<PaymentService.Program>
{
    protected override void BuildConfiguration(
        IConfigurationBuilder configurationBuilder,
        IReadOnlyAccelergreatEnvironmentPipelineData environmentData)
    {
        // Get other service URLs
        var orderServiceUrl = environmentData.GetKestrelWebAppHttpBaseAddress<OrderService.Program>();
        
        configurationBuilder.AddInMemoryCollection(new[]
        {
            new KeyValuePair<string, string>("OrderService:BaseUrl", orderServiceUrl),
            new KeyValuePair<string, string>("PaymentProvider:ApiKey", "test-key")
        });
    }
}
```

---

## 🔧 Advanced Features

### Transaction Overriding (Premium)
Handle nested transactions in your application:

```csharp
builder.ConfigureServices(services =>
{
    // Enable transaction overriding
    services.AddAccelergreatDbContext<OrderDbContext>(
        environmentData,
        useTransactionOverriding: true
    );
});
```

**What it does:**
- Intercepts `BeginTransaction()` calls
- Redirects to savepoints instead of new transactions
- Handles `Commit()` and `Rollback()` automatically

### Database Provider-Specific Extensions
```csharp
// SQL Server specific
services.AddSqlServerDbContextUsingTransaction<OrderDbContext>(
    environmentData,
    options => options.EnableRetryOnFailure(),
    useTransactionOverriding: true
);

// Universal (works with any provider)
services.AddAccelergreatDbContext<OrderDbContext>(
    environmentData,
    useTransactionOverriding: true
);
```

### Global Data Management
```csharp
public class ProductDatabaseComponent : SqlServerEntityFrameworkDatabaseComponent<ProductDbContext>
{
    public ProductDatabaseComponent(IConfiguration configuration) : base(configuration)
    {
        // Auto-register global data entities
        AutoRegisterGlobalDataItemsInDbContextCreation = true;
    }

    protected override async Task OnDatabaseInitializedAsync(ProductDbContext context)
    {
        // This data persists across all tests
        var categories = new[]
        {
            new Category { Name = "Electronics", IsActive = true },
            new Category { Name = "Books", IsActive = true },
            new Category { Name = "Clothing", IsActive = true }
        };
        
        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();
        
        // You can also call complex setup methods
        await SeedComplexTestData(context);
    }
}
```

---

## 🔨 Building Custom Components

### Basic Custom Component
```csharp
public class RedisComponent : IAccelergreatComponent
{
    private ConnectionMultiplexer _redis;
    private IDatabase _database;

    public async Task InitializeAsync(IAccelergreatEnvironmentPipelineData environmentData)
    {
        // Connect to Redis
        _redis = await ConnectionMultiplexer.ConnectAsync("localhost:6379");
        _database = _redis.GetDatabase();
        
        // Make available to other components
        environmentData.Add("RedisConnection", _redis);
        environmentData.Add("RedisDatabase", _database);
        
        // Optional: Add test data
        await _database.StringSetAsync("test:initialized", "true");
    }

    public async Task ResetAsync()
    {
        // Clear Redis between tests
        await _redis.GetServer("localhost:6379").FlushDatabaseAsync();
        
        // Re-add any persistent test data
        await _database.StringSetAsync("test:initialized", "true");
    }

    public async ValueTask DisposeAsync()
    {
        await _redis.DisposeAsync();
    }
}
```

### Advanced Custom Component
```csharp
public class ElasticsearchComponent : IAccelergreatComponent
{
    private ElasticClient _client;
    private readonly string _indexName = "test-index";

    public async Task InitializeAsync(IAccelergreatEnvironmentPipelineData environmentData)
    {
        var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
            .DefaultIndex(_indexName);
        
        _client = new ElasticClient(settings);
        
        // Create index with mapping
        await _client.Indices.CreateAsync(_indexName, c => c
            .Map<Product>(m => m.AutoMap())
        );
        
        // Seed test data
        await SeedTestDocuments();
        
        // Share with other components
        environmentData.Add("ElasticsearchClient", _client);
    }

    public async Task ResetAsync()
    {
        // Clear and re-seed
        await _client.DeleteByQueryAsync<Product>(q => q.MatchAll());
        await SeedTestDocuments();
    }

    private async Task SeedTestDocuments()
    {
        var products = new[]
        {
            new Product { Id = 1, Name = "Test Product 1", Category = "Electronics" },
            new Product { Id = 2, Name = "Test Product 2", Category = "Books" }
        };
        
        await _client.IndexManyAsync(products);
        await _client.Indices.RefreshAsync(_indexName);
    }

    public async ValueTask DisposeAsync()
    {
        await _client.Indices.DeleteAsync(_indexName);
    }
}
```

### Docker Container Component
```csharp
public class DockerDatabaseComponent : IAccelergreatComponent
{
    private string _containerId;
    private readonly string _connectionString;

    public async Task InitializeAsync(IAccelergreatEnvironmentPipelineData environmentData)
    {
        // Start PostgreSQL container
        var startInfo = new ProcessStartInfo
        {
            FileName = "docker",
            Arguments = "run -d -p 5432:5432 -e POSTGRES_PASSWORD=testpass postgres:13",
            UseShellExecute = false,
            RedirectStandardOutput = true
        };
        
        using var process = Process.Start(startInfo);
        _containerId = (await process.StandardOutput.ReadToEndAsync()).Trim();
        
        // Wait for container to be ready
        await WaitForDatabase();
        
        // Share connection string
        environmentData.Add("DockerDbConnectionString", _connectionString);
    }

    public async Task ResetAsync()
    {
        // Reset database by recreating it
        await ExecuteDockerCommand($"exec {_containerId} dropdb -U postgres testdb");
        await ExecuteDockerCommand($"exec {_containerId} createdb -U postgres testdb");
    }

    public async ValueTask DisposeAsync()
    {
        await ExecuteDockerCommand($"stop {_containerId}");
        await ExecuteDockerCommand($"rm {_containerId}");
    }
}
```

---

## 🎯 Best Practices

### 1. Component Ordering
```csharp
// ✅ Correct order - dependencies first
public void Configure(IAccelergreatBuilder builder)
{
    builder.AddAccelergreatComponent<DatabaseComponent>();      // Infrastructure
    builder.AddAccelergreatComponent<CacheComponent>();         // Depends on DB
    builder.AddAccelergreatComponent<ApiComponent>();           // Depends on DB + Cache
    builder.AddAccelergreatComponent<ExternalServiceComponent>(); // Depends on API
}
```

### 2. Use Appropriate Lifetimes
```csharp
// ✅ Good choices
builder.AddAccelergreatComponent<DatabaseComponent>();          // Transient - needs isolation
builder.AddAccelergreatComponent<ApiComponent>();               // Transient - needs fresh state
builder.AddSingletonAccelergreatComponent<ExternalServiceComponent>(); // Singleton - expensive to create

// ❌ Poor choices
builder.AddSingletonAccelergreatComponent<DatabaseComponent>(); // Bad - shared state between tests
builder.AddAccelergreatComponent<ExternalServiceComponent>();   // Bad - unnecessary overhead
```

### 3. Environment-Specific Configuration
```csharp
// Development: Fast feedback
{
  "SqlServerEntityFramework": { "ResetStrategy": "Transactions" }
}

// CI: Reliability
{
  "SqlServerEntityFramework": { "ResetStrategy": "SnapshotRollback" }
}

// Integration: Production-like
{
  "SqlServerEntityFramework": { 
    "ResetStrategy": "SnapshotRollback",
    "ConnectionString": "Server=integration-server;..."
  }
}
```

### 4. Component Communication
```csharp
public class ApiComponent : WebAppComponent<Program>
{
    protected override void BuildConfiguration(
        IConfigurationBuilder configurationBuilder,
        IReadOnlyAccelergreatEnvironmentPipelineData environmentData)
    {
        // ✅ Get data from other components
        var redisConnection = environmentData.Get<ConnectionMultiplexer>("RedisConnection");
        var dbConnectionString = environmentData.GetEntityFrameworkDatabaseConnectionString<AppDbContext>();
        
        configurationBuilder.AddInMemoryCollection(new[]
        {
            new KeyValuePair<string, string>("ConnectionStrings:DefaultConnection", dbConnectionString),
            new KeyValuePair<string, string>("Redis:ConnectionString", redisConnection.ToString())
        });
    }
}
```

---

## 🚀 Performance Optimization

### 1. Use Transactions for Development
```json
{
  "SqlServerEntityFramework": {
    "ResetStrategy": "Transactions"  // 0-3ms resets!
  }
}
```

### 2. Leverage Parallel Execution
```csharp
// Group tests in collections for parallel execution
[Collection("OrderTests")]
public class OrderCreationTests : AccelergreatXunitTest { }

[Collection("OrderTests")]
public class OrderUpdateTests : AccelergreatXunitTest { }

[Collection("ProductTests")]  // Runs in parallel with OrderTests
public class ProductTests : AccelergreatXunitTest { }
```

### 3. Optimize Component Initialization
```csharp
public class OptimizedDatabaseComponent : SqlServerEntityFrameworkDatabaseComponent<DbContext>
{
    protected override async Task OnDatabaseInitializedAsync(DbContext context)
    {
        // ✅ Bulk operations
        context.Products.AddRange(GenerateTestProducts(1000));
        await context.SaveChangesAsync();
        
        // ❌ Individual operations
        // foreach (var product in products)
        // {
        //     context.Products.Add(product);
        //     await context.SaveChangesAsync();
        // }
    }
}
```

---

## 📊 Monitoring & Diagnostics

### Component Performance Metrics
```csharp
// Accelergreat automatically logs:
// - Component initialization time
// - Reset operation duration
// - Environment allocation/deallocation
// - Parallel execution statistics
```

### Example Log Output
```
[INFO] Initialized environment [1] in 234ms
[INFO] Environment [1] allocated.
[INFO] Reset environment [1] in 2ms.
[INFO] Environment [2] allocated.
[INFO] Initialized 4 environments in 1.2s
```

---

## 📚 Next Steps

- 🎯 **Try the Examples**: [Sample Projects](https://github.com/Accelergreat/Examples)
- 🏢 **Microservices**: Read the [Microservices Guide](microservices.md)
- 💬 **Get Help**: Join our [Discord Community](https://discord.com/channels/1175044305988091995/1175044307032481804)
- 🔧 **Advanced Usage**: Explore the [API Reference](https://docs.accelergreat.net/api/)

---

> **📝 Documentation Notice**: This documentation has been generated using Cursor AI to improve clarity and developer experience. While every effort has been made to proofread and ensure accuracy, if you encounter any issues or inaccuracies, please contact us at [mail@accelergreat.net](mailto:mail@accelergreat.net).

*Made with ❤️ by developers, for developers. Components make your integration tests fast, reliable, and maintainable.*