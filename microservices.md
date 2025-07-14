# 🏢 Microservices Testing Guide

Test complex microservice architectures with confidence. Accelergreat enables you to run multiple services together, test service-to-service communication, and debug across your entire distributed system.

> **Advanced Feature**: Microservices testing showcases Accelergreat's powerful capabilities for complex integration scenarios.

## 🎯 What You'll Learn

- 🔧 **Multi-Service Setup** - Run multiple APIs together
- 🌐 **Service Communication** - Test API-to-API calls
- 🔍 **Cross-Service Debugging** - Debug across your entire system
- 🚀 **Advanced Scenarios** - Authentication, message queues, event sourcing

---

## 🚀 Quick Start

### Basic E-Commerce Example
```csharp
public class ECommerceIntegrationTests : AccelergreatXunitTest
{
    public ECommerceIntegrationTests(IAccelergreatEnvironmentPool environmentPool) : base(environmentPool)
    {
    }

    [Fact]
    public async Task PlaceOrder_ShouldProcessPaymentAndNotifyInventory()
    {
        // All services running together!
        var orderClient = GetComponent<OrderServiceComponent>().CreateClient();
        var paymentClient = GetComponent<PaymentServiceComponent>().CreateClient();
        var inventoryClient = GetComponent<InventoryServiceComponent>().CreateClient();
        
        // Test cross-service workflow
        var order = await orderClient.PostAsJsonAsync("/orders", new
        {
            ProductId = 123,
            Quantity = 2,
            CustomerId = 456
        });
        
        // Verify service interactions
        order.Should().BeSuccessful();
        
        // Payment service was called
        var payment = await paymentClient.GetAsync($"/payments/order/{order.Id}");
        payment.Should().BeSuccessful();
        
        // Inventory was updated
        var inventory = await inventoryClient.GetAsync("/inventory/123");
        var stock = await inventory.Content.ReadFromJsonAsync<InventoryItem>();
        stock.Quantity.Should().Be(8); // Started with 10, ordered 2
    }
}
```

---

## 🔧 Setup Guide

### 1. Project References
Add references to all your microservice projects:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\..\src\OrderService\OrderService.csproj" />
    <ProjectReference Include="..\..\src\PaymentService\PaymentService.csproj" />
    <ProjectReference Include="..\..\src\InventoryService\InventoryService.csproj" />
    <ProjectReference Include="..\..\src\NotificationService\NotificationService.csproj" />
  </ItemGroup>
</Project>
```

### 2. Aliases for Minimal APIs (.NET 6+)
For projects using minimal APIs, add aliases to avoid naming conflicts:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\..\src\OrderService\OrderService.csproj" Aliases="OrderService" />
    <ProjectReference Include="..\..\src\PaymentService\PaymentService.csproj" Aliases="PaymentService" />
    <ProjectReference Include="..\..\src\InventoryService\InventoryService.csproj" Aliases="InventoryService" />
  </ItemGroup>
</Project>
```

### 3. Component Creation
Create `KestrelWebAppComponent` for each service:

```csharp
// Order Service Component
extern alias OrderService;
using OrderProgram = OrderService::Program;

public class OrderServiceComponent : KestrelWebAppComponent<OrderProgram>
{
    protected override void BuildConfiguration(
        IConfigurationBuilder configurationBuilder,
        IReadOnlyAccelergreatEnvironmentPipelineData environmentData)
    {
        // Database connection
        configurationBuilder.AddEntityFrameworkDatabaseConnectionString<OrderDbContext>(
            "DefaultConnection", environmentData);
        
        // Other service URLs
        var paymentServiceUrl = environmentData.GetKestrelWebAppHttpBaseAddress<PaymentService.Program>();
        var inventoryServiceUrl = environmentData.GetKestrelWebAppHttpBaseAddress<InventoryService.Program>();
        
        configurationBuilder.AddInMemoryCollection(new[]
        {
            new KeyValuePair<string, string>("PaymentService:BaseUrl", paymentServiceUrl),
            new KeyValuePair<string, string>("InventoryService:BaseUrl", inventoryServiceUrl)
        });
    }
}

// Payment Service Component
extern alias PaymentService;
using PaymentProgram = PaymentService::Program;

public class PaymentServiceComponent : KestrelWebAppComponent<PaymentProgram>
{
    protected override void BuildConfiguration(
        IConfigurationBuilder configurationBuilder,
        IReadOnlyAccelergreatEnvironmentPipelineData environmentData)
    {
        configurationBuilder.AddEntityFrameworkDatabaseConnectionString<PaymentDbContext>(
            "DefaultConnection", environmentData);
        
        // External payment provider (mock)
        configurationBuilder.AddInMemoryCollection(new[]
        {
            new KeyValuePair<string, string>("PaymentProvider:ApiKey", "test-key-123"),
            new KeyValuePair<string, string>("PaymentProvider:BaseUrl", "https://api.mockpayment.com")
        });
    }
}

// Inventory Service Component
extern alias InventoryService;
using InventoryProgram = InventoryService::Program;

public class InventoryServiceComponent : KestrelWebAppComponent<InventoryProgram>
{
    protected override void BuildConfiguration(
        IConfigurationBuilder configurationBuilder,
        IReadOnlyAccelergreatEnvironmentPipelineData environmentData)
    {
        configurationBuilder.AddEntityFrameworkDatabaseConnectionString<InventoryDbContext>(
            "DefaultConnection", environmentData);
    }
}
```

### 4. Startup Configuration
```csharp
public class Startup : IAccelergreatStartup
{
    public void Configure(IAccelergreatBuilder builder)
    {
        // Database first
        builder.AddAccelergreatComponent<DatabaseComponent>();
        
        // Services in dependency order
        builder.AddAccelergreatComponent<InventoryServiceComponent>();
        builder.AddAccelergreatComponent<PaymentServiceComponent>();
        builder.AddAccelergreatComponent<OrderServiceComponent>();  // Depends on others
        
        // Optional: Notification service
        builder.AddAccelergreatComponent<NotificationServiceComponent>();
    }
}
```

---

## 🎨 Advanced Scenarios

### Authentication & Authorization
```csharp
public class AuthServiceComponent : KestrelWebAppComponent<AuthService.Program>
{
    protected override void BuildConfiguration(
        IConfigurationBuilder configurationBuilder,
        IReadOnlyAccelergreatEnvironmentPipelineData environmentData)
    {
        configurationBuilder.AddInMemoryCollection(new[]
        {
            new KeyValuePair<string, string>("JWT:SecretKey", "super-secret-test-key-1234567890"),
            new KeyValuePair<string, string>("JWT:Issuer", "TestIssuer"),
            new KeyValuePair<string, string>("JWT:Audience", "TestAudience")
        });
    }
}

public class OrderServiceComponent : KestrelWebAppComponent<OrderService.Program>
{
    protected override void BuildConfiguration(
        IConfigurationBuilder configurationBuilder,
        IReadOnlyAccelergreatEnvironmentPipelineData environmentData)
    {
        // Get auth service URL
        var authServiceUrl = environmentData.GetKestrelWebAppHttpBaseAddress<AuthService.Program>();
        
        configurationBuilder.AddInMemoryCollection(new[]
        {
            new KeyValuePair<string, string>("Authentication:Authority", authServiceUrl),
            new KeyValuePair<string, string>("Authentication:Audience", "TestAudience")
        });
    }
}
```

### Message Queues & Event Sourcing
```csharp
public class MessageQueueComponent : IAccelergreatComponent
{
    private readonly InMemoryMessageQueue _messageQueue = new();

    public async Task InitializeAsync(IAccelergreatEnvironmentPipelineData environmentData)
    {
        // Share message queue across services
        environmentData.Add("MessageQueue", _messageQueue);
        
        // Configure message handlers
        _messageQueue.Subscribe<OrderPlacedEvent>(HandleOrderPlaced);
        _messageQueue.Subscribe<PaymentProcessedEvent>(HandlePaymentProcessed);
    }

    public Task ResetAsync()
    {
        _messageQueue.Clear();
        return Task.CompletedTask;
    }

    private Task HandleOrderPlaced(OrderPlacedEvent @event)
    {
        // Test event handling
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

public class OrderServiceComponent : KestrelWebAppComponent<OrderService.Program>
{
    protected override void BuildConfiguration(
        IConfigurationBuilder configurationBuilder,
        IReadOnlyAccelergreatEnvironmentPipelineData environmentData)
    {
        var messageQueue = environmentData.Get<InMemoryMessageQueue>("MessageQueue");
        
        configurationBuilder.AddInMemoryCollection(new[]
        {
            new KeyValuePair<string, string>("MessageQueue:Type", "InMemory")
        });
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder,
        IConfiguration configuration,
        IReadOnlyAccelergreatEnvironmentPipelineData environmentData)
    {
        builder.ConfigureServices(services =>
        {
            var messageQueue = environmentData.Get<InMemoryMessageQueue>("MessageQueue");
            services.AddSingleton(messageQueue);
        });
    }
}
```

### API Gateway Integration
```csharp
public class ApiGatewayComponent : KestrelWebAppComponent<ApiGateway.Program>
{
    protected override void BuildConfiguration(
        IConfigurationBuilder configurationBuilder,
        IReadOnlyAccelergreatEnvironmentPipelineData environmentData)
    {
        // Configure downstream services
        var orderServiceUrl = environmentData.GetKestrelWebAppHttpBaseAddress<OrderService.Program>();
        var paymentServiceUrl = environmentData.GetKestrelWebAppHttpBaseAddress<PaymentService.Program>();
        var inventoryServiceUrl = environmentData.GetKestrelWebAppHttpBaseAddress<InventoryService.Program>();
        
        configurationBuilder.AddInMemoryCollection(new[]
        {
            new KeyValuePair<string, string>("DownstreamServices:OrderService", orderServiceUrl),
            new KeyValuePair<string, string>("DownstreamServices:PaymentService", paymentServiceUrl),
            new KeyValuePair<string, string>("DownstreamServices:InventoryService", inventoryServiceUrl)
        });
    }
}
```

---

## 🔍 Testing Patterns

### End-to-End Workflow Testing
```csharp
[Fact]
public async Task CompleteOrderWorkflow_ShouldUpdateAllServices()
{
    // Arrange
    var gatewayClient = GetComponent<ApiGatewayComponent>().CreateClient();
    var orderDb = GetComponent<OrderDatabaseComponent>().DbContextFactory.NewDbContext();
    var inventoryDb = GetComponent<InventoryDatabaseComponent>().DbContextFactory.NewDbContext();
    
    // Setup test data
    var product = new Product { Id = 1, Name = "Test Product", Price = 99.99m };
    var inventory = new InventoryItem { ProductId = 1, Quantity = 10 };
    
    inventoryDb.Products.Add(product);
    inventoryDb.InventoryItems.Add(inventory);
    await inventoryDb.SaveChangesAsync();
    
    // Act - Place order through gateway
    var response = await gatewayClient.PostAsJsonAsync("/api/orders", new
    {
        ProductId = 1,
        Quantity = 2,
        CustomerId = 123
    });
    
    // Assert - Check all services were updated
    response.Should().BeSuccessful();
    
    var order = await response.Content.ReadFromJsonAsync<Order>();
    order.Should().NotBeNull();
    order.Status.Should().Be(OrderStatus.Confirmed);
    
    // Verify order was saved
    var savedOrder = await orderDb.Orders.FindAsync(order.Id);
    savedOrder.Should().NotBeNull();
    
    // Verify inventory was updated
    var updatedInventory = await inventoryDb.InventoryItems.FindAsync(1);
    updatedInventory.Quantity.Should().Be(8);
    
    // Verify payment was processed
    var paymentClient = GetComponent<PaymentServiceComponent>().CreateClient();
    var paymentResponse = await paymentClient.GetAsync($"/payments/order/{order.Id}");
    paymentResponse.Should().BeSuccessful();
}
```

### Service Communication Testing
```csharp
[Fact]
public async Task OrderService_ShouldCallPaymentService_WhenOrderPlaced()
{
    // Arrange
    var orderClient = GetComponent<OrderServiceComponent>().CreateClient();
    var paymentClient = GetComponent<PaymentServiceComponent>().CreateClient();
    
    // Mock payment service endpoint
    var mockPaymentHandler = new MockPaymentHandler();
    var paymentService = GetComponent<PaymentServiceComponent>();
    paymentService.ConfigureTestServices(services =>
    {
        services.AddSingleton(mockPaymentHandler);
    });
    
    // Act
    var orderResponse = await orderClient.PostAsJsonAsync("/orders", new
    {
        ProductId = 1,
        Quantity = 1,
        CustomerId = 123,
        Amount = 99.99m
    });
    
    // Assert
    orderResponse.Should().BeSuccessful();
    
    // Verify payment service was called
    mockPaymentHandler.PaymentRequests.Should().HaveCount(1);
    mockPaymentHandler.PaymentRequests[0].Amount.Should().Be(99.99m);
}
```

### Circuit Breaker & Resilience Testing
```csharp
[Fact]
public async Task OrderService_ShouldHandlePaymentServiceFailure()
{
    // Arrange
    var orderClient = GetComponent<OrderServiceComponent>().CreateClient();
    
    // Simulate payment service failure
    var paymentService = GetComponent<PaymentServiceComponent>();
    paymentService.SimulateFailure(HttpStatusCode.ServiceUnavailable);
    
    // Act
    var orderResponse = await orderClient.PostAsJsonAsync("/orders", new
    {
        ProductId = 1,
        Quantity = 1,
        CustomerId = 123,
        Amount = 99.99m
    });
    
    // Assert - Order should be created but marked as pending
    orderResponse.Should().BeSuccessful();
    
    var order = await orderResponse.Content.ReadFromJsonAsync<Order>();
    order.Status.Should().Be(OrderStatus.PaymentPending);
}
```

---

## 🐛 Debugging Across Services

### Visual Studio Debugging
```csharp
[Fact]
public async Task DebugAcrossServices()
{
    // Set breakpoints in any service!
    var orderClient = GetComponent<OrderServiceComponent>().CreateClient();
    var paymentClient = GetComponent<PaymentServiceComponent>().CreateClient();
    
    // Breakpoint here
    var order = await orderClient.PostAsJsonAsync("/orders", new { ... });
    
    // Step through OrderService code
    // Step through PaymentService code
    // Debug the entire workflow!
}
```

### Logging & Diagnostics
```csharp
public class OrderServiceComponent : KestrelWebAppComponent<OrderService.Program>
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder,
        IConfiguration configuration,
        IReadOnlyAccelergreatEnvironmentPipelineData environmentData)
    {
        builder.ConfigureServices(services =>
        {
            // Enhanced logging for debugging
            services.AddLogging(config =>
            {
                config.AddConsole();
                config.AddDebug();
                config.SetMinimumLevel(LogLevel.Debug);
            });
        });
    }
}
```

---

## 📊 Performance Considerations

### Service Startup Optimization
```csharp
public class OptimizedServiceComponent : KestrelWebAppComponent<Service.Program>
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder,
        IConfiguration configuration,
        IReadOnlyAccelergreatEnvironmentPipelineData environmentData)
    {
        builder.ConfigureServices(services =>
        {
            // Disable unnecessary services in tests
            services.Configure<HealthCheckOptions>(options =>
            {
                options.Enabled = false;
            });
            
            // Use in-memory caching instead of Redis
            services.AddMemoryCache();
            // services.AddStackExchangeRedisCache(...); // Disabled for tests
        });
    }
}
```

### Database Sharing
```csharp
public class SharedDatabaseComponent : SqlServerEntityFrameworkDatabaseComponent<SharedDbContext>
{
    public SharedDatabaseComponent(IConfiguration configuration) : base(configuration)
    {
    }
}

public class OrderServiceComponent : KestrelWebAppComponent<OrderService.Program>
{
    protected override void BuildConfiguration(
        IConfigurationBuilder configurationBuilder,
        IReadOnlyAccelergreatEnvironmentPipelineData environmentData)
    {
        // Multiple services can share the same database
        configurationBuilder.AddEntityFrameworkDatabaseConnectionString<SharedDbContext>(
            "DefaultConnection", environmentData);
    }
}
```

---

## 🎯 Best Practices

### 1. Service Dependency Order
```csharp
public void Configure(IAccelergreatBuilder builder)
{
    // ✅ Correct order - foundation services first
    builder.AddAccelergreatComponent<DatabaseComponent>();
    builder.AddAccelergreatComponent<CacheComponent>();
    builder.AddAccelergreatComponent<MessageQueueComponent>();
    
    // Core services
    builder.AddAccelergreatComponent<AuthServiceComponent>();
    builder.AddAccelergreatComponent<InventoryServiceComponent>();
    builder.AddAccelergreatComponent<PaymentServiceComponent>();
    
    // Composite services
    builder.AddAccelergreatComponent<OrderServiceComponent>();
    builder.AddAccelergreatComponent<NotificationServiceComponent>();
    
    // Gateway last
    builder.AddAccelergreatComponent<ApiGatewayComponent>();
}
```

### 2. Configuration Management
```csharp
// ✅ Use environment data for service URLs
var paymentServiceUrl = environmentData.GetKestrelWebAppHttpBaseAddress<PaymentService.Program>();

// ❌ Don't hardcode URLs
var paymentServiceUrl = "https://localhost:5001";
```

### 3. Test Isolation
```csharp
[Collection("ECommerceTests")]
public class OrderTests : AccelergreatXunitTest
{
    // Tests in same collection share services
}

[Collection("ECommerceTests")]
public class PaymentTests : AccelergreatXunitTest
{
    // Same service instances
}

[Collection("InventoryTests")]  // Different collection = different instances
public class InventoryTests : AccelergreatXunitTest
{
    // Fresh service instances
}
```

### 4. Mock External Dependencies
```csharp
public class PaymentServiceComponent : KestrelWebAppComponent<PaymentService.Program>
{
    protected override void BuildConfiguration(
        IConfigurationBuilder configurationBuilder,
        IReadOnlyAccelergreatEnvironmentPipelineData environmentData)
    {
        // ✅ Mock external services
        configurationBuilder.AddInMemoryCollection(new[]
        {
            new KeyValuePair<string, string>("PaymentProvider:BaseUrl", "https://mock.payment.com"),
            new KeyValuePair<string, string>("PaymentProvider:ApiKey", "test-key")
        });
    }
}
```

---

## 📚 Real-World Examples

### E-Commerce Platform
```csharp
// Services: OrderService, PaymentService, InventoryService, NotificationService
// Database: Shared SQL Server with separate schemas
// Communication: HTTP REST APIs
// Authentication: JWT tokens via AuthService
```

### Event-Driven Architecture
```csharp
// Services: OrderService, PaymentService, InventoryService
// Communication: Event bus (RabbitMQ/Azure Service Bus)
// Database: Event store + read models
// Patterns: CQRS, Event Sourcing
```

### Microservices with Gateway
```csharp
// Services: 10+ microservices
// Gateway: Ocelot/YARP API Gateway
// Database: Multiple databases per service
// Discovery: Service discovery with health checks
```

---

## 🔗 Related Resources

- 📖 [Main Documentation](index.md)
- 🧩 [Components Guide](components.md)
- 💻 [Microservices Examples](https://github.com/Accelergreat/Examples/tree/main/Microservices)
- 🎯 [Best Practices](https://github.com/Accelergreat/Examples/tree/main/BestPractices)

---

## 🎉 Community Support

Need help with complex microservices scenarios? Get support from our community:
- **Discord Community** - Active developer community
- **GitHub Issues** - Bug reports and feature requests
- **Documentation** - Comprehensive guides and examples
- **Email Support** - General questions and guidance

**Contact**: [mail@accelergreat.net](mailto:mail@accelergreat.net) • [Discord](https://discord.com/channels/1175044305988091995/1175044307032481804)

---

> **📝 Documentation Notice**: This documentation has been generated using Cursor AI to improve clarity and developer experience. While every effort has been made to proofread and ensure accuracy, if you encounter any issues or inaccuracies, please contact us at [mail@accelergreat.net](mailto:mail@accelergreat.net).

*Master microservices testing with Accelergreat. Turn complex distributed systems into simple, fast, reliable tests.*