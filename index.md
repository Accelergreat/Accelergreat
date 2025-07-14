---
uid: DeveloperDocumentation.Index
---

# ⚡ Accelergreat 4.0 - Ultra Fast Integration Tests

**The fastest way to write and run integration tests in .NET**

Stop fighting with slow, brittle integration tests. Accelergreat automatically manages your test dependencies, runs tests in parallel, and resets databases in milliseconds. Focus on writing great tests, not test infrastructure.

## 🚀 Why Developers Love Accelergreat

### Lightning Fast Database Resets
- **Transactions Mode**: 0-3ms database resets using savepoints
- **Parallel Execution**: Tests run simultaneously across multiple environments
- **Smart Pooling**: Intelligent resource management scales with your machine

### Zero Boilerplate
- **Auto-managed Dependencies**: Databases, APIs, and services configured automatically
- **Clean Test Code**: No setup/teardown code cluttering your tests
- **Familiar APIs**: Works with your existing xUnit knowledge

### Production-Ready
- **Multiple Database Providers**: SQL Server, SQLite 
- **Microservices Support**: Test complex multi-service scenarios
- **Environment Configuration**: Development and CI configs

---

## 🎯 Quick Start (2 minutes)

### 1. Install Accelergreat
```bash
    dotnet add package Accelergreat.Xunit
dotnet add package Accelergreat.EntityFramework.SqlServer
```

### 2. Configure Components
```csharp
public class Startup : IAccelergreatStartup
{
    public void Configure(IAccelergreatBuilder builder)
    {
        builder.AddAccelergreatComponent<ProductDatabaseComponent>();
        builder.AddAccelergreatComponent<ProductApiComponent>();
    }
}

public class ProductDatabaseComponent : SqlServerEntityFrameworkDatabaseComponent<ProductDbContext>
{
    public ProductDatabaseComponent(IConfiguration configuration) : base(configuration)
    {
    }
}
```

### 3. Create Your First Test
```csharp
public class ProductTests : AccelergreatXunitTest
{
    public ProductTests(IAccelergreatEnvironmentPool environmentPool) : base(environmentPool)
    {
    }

    [Fact]
    public async Task CreateProduct_ShouldPersistToDatabase()
    {
        // Arrange - Get auto-managed database
        var dbContext = GetComponent<ProductDatabaseComponent>().DbContextFactory.NewDbContext();
        var product = new Product { Name = "Test Product", Price = 99.99m };

        // Act
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        // Assert
        var saved = await dbContext.Products.FindAsync(product.Id);
        saved.Should().NotBeNull();
        saved.Name.Should().Be("Test Product");
        
        // Database automatically resets after each test!
    }
}
```

### 4. Run Tests
```bash
dotnet test
```

That's it! Your tests now run in parallel with ultra-fast database resets.

---

## 🏗️ NuGet Packages

[![NuGet](https://img.shields.io/badge/nuget-v4.0.0-blue)](https://www.nuget.org/profiles/Nanogunn) ![.NET](https://img.shields.io/badge/.NET-6%20%7C%207%20%7C%208%20%7C%209-purple)

### Core Packages
| Package | Description |
|---------|-------------|
| **[Accelergreat.Xunit](https://www.nuget.org/packages/Accelergreat.Xunit)** | xUnit integration & test framework |
| **[Accelergreat](https://www.nuget.org/packages/Accelergreat)** | Core package for custom components |

### Database Packages
| Database | Package |
|----------|---------|
| **SQL Server** | [Accelergreat.EntityFramework.SqlServer](https://www.nuget.org/packages/Accelergreat.EntityFramework.SqlServer) |
| **SQLite** | [Accelergreat.EntityFramework.Sqlite](https://www.nuget.org/packages/Accelergreat.EntityFramework.Sqlite) |


### Application Packages
| Type | Package |
|------|---------|
| **Web APIs** | [Accelergreat.Web](https://www.nuget.org/packages/Accelergreat.Web) |

> **✨ New in v4.0**: Full .NET 9 support, enhanced performance, improved diagnostics

---

## 🧩 Component Architecture

Components are the heart of Accelergreat. They represent your test dependencies and handle all the heavy lifting.

### Database Components

#### SQL Server with Lightning-Fast Resets
```csharp
public class OrderDatabaseComponent : SqlServerEntityFrameworkDatabaseComponent<OrderDbContext>
{
    public OrderDatabaseComponent(IConfiguration configuration) : base(configuration)
    {
    }

    // Optional: Add global test data
    protected override async Task OnDatabaseInitializedAsync(OrderDbContext context)
    {
        context.Categories.Add(new Category { Name = "Electronics" });
        await context.SaveChangesAsync();
    }
}
```

#### Configuration for Different Environments
```json
// accelergreat.development.json
{
  "SqlServerEntityFramework": {
    "ResetStrategy": "Transactions",  // 0-3ms resets!
    "CreateStrategy": "Migrations"
  }
}

// accelergreat.ci.json  
{
  "SqlServerEntityFramework": {
    "ResetStrategy": "SnapshotRollback",  // 80-150ms resets
    "ConnectionString": "Server=ci-server;Database=TestDb;..."
  }
}
```

### Web API Components

#### Test Your APIs Effortlessly
```csharp
public class OrderApiComponent : WebAppComponent<OrderApi.Startup>
{
    protected override void BuildConfiguration(
        IConfigurationBuilder configurationBuilder,
        IReadOnlyAccelergreatEnvironmentPipelineData environmentData)
    {
        // Auto-inject database connection
        configurationBuilder.AddEntityFrameworkDatabaseConnectionString<OrderDbContext>(
            "DefaultConnection", environmentData);
    }
}
```

#### Modern .NET 6+ Program.cs Support
```csharp
public class OrderApiComponent : WebAppComponent<Program>
{
    protected override void BuildConfiguration(
        IConfigurationBuilder configurationBuilder,
        IReadOnlyAccelergreatEnvironmentPipelineData environmentData)
    {
        configurationBuilder.AddEntityFrameworkDatabaseConnectionString<OrderDbContext>(
            "DefaultConnection", environmentData);
    }
}
```

### Microservices Components
```csharp
public class PaymentServiceComponent : KestrelWebAppComponent<PaymentService.Program>
{
    protected override void BuildConfiguration(
            IConfigurationBuilder configurationBuilder,
        IReadOnlyAccelergreatEnvironmentPipelineData environmentData)
    {
        var orderServiceUrl = environmentData.GetKestrelWebAppHttpBaseAddress<OrderService.Program>();
        configurationBuilder.AddInMemoryCollection(new[] {
            new KeyValuePair<string, string>("OrderService:BaseUrl", orderServiceUrl)
        });
    }
}
```

---

## ⚡ Performance Features

### 1. Parallel Test Execution
Accelergreat works with xUnit's parallel execution through intelligent environment pooling:

```json
// xunit.runner.json
{
  "maxParallelThreads": 4,
  "parallelizeTestCollections": true
}
```

**Results**: Up to 5x faster test execution on multi-core machines!

### 2. Ultra-Fast Database Resets

#### Transaction Mode - 0-3ms
```json
{
  "SqlServerEntityFramework": {
    "ResetStrategy": "Transactions"
  }
}
```
Uses savepoints for instant rollbacks. Perfect for development.

#### Snapshot Mode - 80-150ms
```json
{
  "SqlServerEntityFramework": {
    "ResetStrategy": "SnapshotRollback"
  }
}
```
Creates database snapshots for reliable resets. Great for CI.

### 3. Environment Pooling
```csharp
// Automatically manages test environments
- Environment [1] allocated  
- Environment [2] allocated
- Environment [3] allocated
// Tests run in parallel across environments
```

---

## 🔧 Advanced Features

### Transaction Overriding
Handle nested transactions in your application code:

```csharp
builder.ConfigureServices(services =>
{
    services.AddAccelergreatDbContext<OrderDbContext>(
        environmentData, 
        useTransactionOverriding: true  // Handles nested transactions
    );
});
```

### Custom Components
Build your own components for specific needs:

```csharp
public class RedisComponent : IAccelergreatComponent
{
    private ConnectionMultiplexer _redis;

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

### Environment-Based Configuration
```csharp
// Set environment via environment variable
Environment.SetEnvironmentVariable("ACCELERGREAT_ENVIRONMENT", "development");

// Or configure via user secrets
dotnet user-secrets set "ACCELERGREAT:SqlServerEntityFramework:ResetStrategy" "Transactions"
```

---

## 🎨 Complete Example

Here's a full example showing Accelergreat's power:

```csharp
public class ECommerceIntegrationTests : AccelergreatXunitTest
{
    public ECommerceIntegrationTests(IAccelergreatEnvironmentPool environmentPool) : base(environmentPool)
    {
    }

    [Fact]
    public async Task PlaceOrder_ShouldProcessPaymentAndUpdateInventory()
    {
        // Arrange
        var dbContext = GetComponent<ECommerceDatabaseComponent>().DbContextFactory.NewDbContext();
        var httpClient = GetComponent<ECommerceApiComponent>().CreateClient();
        
        // Add test product
        var product = new Product { Name = "Gaming Laptop", Price = 1299.99m, Stock = 10 };
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/orders", new
        {
            ProductId = product.Id,
            Quantity = 2,
            CustomerEmail = "test@example.com"
        });

        // Assert
        response.Should().BeSuccessful();
        
        var order = await response.Content.ReadFromJsonAsync<Order>();
        order.Should().NotBeNull();
        order.Total.Should().Be(2599.98m);
        
        // Verify database changes
        var updatedProduct = await dbContext.Products.FindAsync(product.Id);
        updatedProduct.Stock.Should().Be(8);  // Stock decreased
        
        var savedOrder = await dbContext.Orders.FindAsync(order.Id);
        savedOrder.Should().NotBeNull();
        savedOrder.Status.Should().Be(OrderStatus.Processing);
    }
}

public class Startup : IAccelergreatStartup
{
    public void Configure(IAccelergreatBuilder builder)
    {
        builder.AddAccelergreatComponent<ECommerceDatabaseComponent>();
        builder.AddAccelergreatComponent<ECommerceApiComponent>();
    }
}
```

---

## 🚀 Migration Guide

### From Traditional Integration Tests
```csharp
// Before: Manual Entity Framework setup/teardown
public class OrderTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    
    public OrderTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task Test()
    {
        using var dbContext = _fixture.CreateDbContext();
        
        // Manual cleanup before test
        dbContext.Orders.RemoveRange(dbContext.Orders);
        dbContext.Products.RemoveRange(dbContext.Products);
        await dbContext.SaveChangesAsync();
        
        // Test code
        var product = new Product { Name = "Test" };
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();
        
        // Manual cleanup after test (or risk affecting other tests)
        dbContext.Orders.RemoveRange(dbContext.Orders);
        dbContext.Products.RemoveRange(dbContext.Products);
        await dbContext.SaveChangesAsync();
    }
}

// After: Accelergreat handles everything
[Fact]
public async Task Test()
{
    var dbContext = GetComponent<TestDatabaseComponent>().DbContextFactory.NewDbContext();
    
    // Test code - database auto-resets between tests!
    var product = new Product { Name = "Test" };
    dbContext.Products.Add(product);
    await dbContext.SaveChangesAsync();
    
    // No cleanup needed - next test gets fresh database
}
```

### From WebApplicationFactory
```csharp
// Before: Manual WebApplicationFactory
public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    
    public ApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
}

// After: Accelergreat component
public class ApiTests : AccelergreatXunitTest
{
    public ApiTests(IAccelergreatEnvironmentPool environmentPool) : base(environmentPool)
    {
    }
    
    [Fact]
    public async Task Test()
    {
        var client = GetComponent<ApiComponent>().CreateClient();
        // Test with auto-configured database!
    }
}
```

---

## 🎯 Best Practices

### 1. Component Order Matters
```csharp
public void Configure(IAccelergreatBuilder builder)
{
    builder.AddAccelergreatComponent<DatabaseComponent>();  // First
    builder.AddAccelergreatComponent<ApiComponent>();       // Second (depends on DB)
}
```

### 2. Use Environment-Specific Configuration
```csharp
// Development: Fast transactions
// CI: Reliable snapshots
```

### 3. Configure Parallel Execution
```json
// xunit.runner.json
{
  "maxParallelThreads": 4,
  "parallelizeTestCollections": true
}
```

```csharp
// Tests automatically run in parallel - no collections needed!
public class OrderCreationTests : AccelergreatXunitTest { }
public class OrderUpdateTests : AccelergreatXunitTest { }
public class ProductTests : AccelergreatXunitTest { }

// Optional: Use collections only for logical grouping
[Collection("SlowTests")]
public class LongRunningTests : AccelergreatXunitTest { }
```

### 4. Debugging Tips
```csharp
// ⚠️ Important: In Visual Studio, don't click "Stop" during debugging
// Let tests complete naturally for proper cleanup
```

---




---

## 📚 Resources

### Documentation
- 📖 [Getting Started Guide](https://docs.accelergreat.net/)
- 🔧 [API Reference](https://docs.accelergreat.net/api/)
- 📝 [Component Guide](components.md)
- 🏢 [Microservices Guide](microservices.md)

### Examples
- 💻 [Sample Projects](https://github.com/Accelergreat/Examples)


### Community
- 💬 [Discord Community](https://discord.com/channels/1175044305988091995/1175044307032481804)
- 📧 [Email Support](mailto:mail@accelergreat.net)
- 🐛 [GitHub Issues](https://github.com/Accelergreat/Accelergreat/issues)
- ❓ [FAQ](https://accelergreat.net/faqs)

---

## 📄 License

**Accelergreat is completely free to use** - No licensing fees, no restrictions, all features included.

**All Features Available:**
- Transaction reset strategy
- Microservices support  
- Database providers
- Parallel execution
- Environment pooling

---

> **📝 Documentation Notice**: This documentation has been generated using Cursor AI to improve clarity and developer experience. While every effort has been made to proofread and ensure accuracy, if you encounter any issues or inaccuracies, please contact us at [mail@accelergreat.net](mailto:mail@accelergreat.net).

*Made with ❤️ by developers, for developers. Transform your integration testing experience with Accelergreat 4.0.*
