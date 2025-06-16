
# Components

One of the core features of Accelergreat is the concept of components. A component represents a dependency for your tests. For example a database or a web API.

If you haven't already, please read the [main documentation](index.md).

## Initialization

Components are initialized at the beginning of a test assembly execution, before any tests start executing.

```csharp
[assembly: UseAccelergreatXunitTestFramework]

namespace ExampleTestProject;

public class Startup : IAccelergreatStartup
{
    public void ConfigureServices(IServiceCollection  services)
    {
        services.AddSingletonAccelergreatComponent<C1>();

        services.AddAccelergreatComponent<C2>();

        services.AddAccelergreatComponent<C3>();
    }
}
```

**Order is important**

Components are initialized in the order they are registered within the class that implements [`IAccelergreatStartup.ConfigureServices`](xref:Accelergreat.Xunit.IAccelergreatStartup.ConfigureServices(Microsoft.Extensions.DependencyInjection.IServiceCollection)).

| **Component** | **C1** | **C2** | **C3** |
|--|--|--|--|
| **Consumes** | | data[0], data[1] |data[2] |
| **Adds** | data[0], data[1] | data[2] | |

## Lifetime

### Transient

A component is registered as transient by calling [`IServiceCollection.AddAccelergreatComponent`](xref:Accelergreat.Xunit.Extensions.ServiceCollectionExtensions.AddAccelergreatComponent``1(Microsoft.Extensions.DependencyInjection.IServiceCollection)) or [`IServiceCollection.AddTransientAccelergreatComponent`](xref:Accelergreat.Xunit.Extensions.ServiceCollectionExtensions.AddTransientAccelergreatComponent``1(Microsoft.Extensions.DependencyInjection.IServiceCollection)). This means 1 component instance per environment. If you have 8 logical processors and at least 8 test collections to warrant 8 parallel environments, there will be 8 instances of the component.

### Singleton

A component is registered as singleton by calling [`IServiceCollection.AddSingletonAccelergreatComponent`](xref:Accelergreat.Xunit.Extensions.ServiceCollectionExtensions.AddSingletonAccelergreatComponent``1(Microsoft.Extensions.DependencyInjection.IServiceCollection)). This means 1 component instance per assembly. If you have 8 logical processors and at least 8 test collections to warrant 8 parallel environments, there will be 1 instance of the component shared between 8 environments.

When a component is registered as singleton, the [`IAccelergreatComponent.ResetAsync`](xref:Accelergreat.Components.IAccelergreatComponent.ResetAsync) method is not called.

## Pre-built components

### Databases (Entity Framework)

#### SQL Server


**Package** [Accelergreat.EntityFramework.SqlServer](https://www.nuget.org/packages/Accelergreat.EntityFramework.SqlServer)

**Class** [`SqlServerEntityFrameworkDatabaseComponent`](xref:Accelergreat.EntityFramework.SqlServer.SqlServerEntityFrameworkDatabaseComponent`1)

**Configuration schema** see the `SqlServerEntityFramework` section in [schema.json](https://cdn.accelergreat.net/configuration/~(version)~/schema.json)

**Reset strategies**

 - SnapshotRollback (default)
 Executes `CREATE DATABASE AS SNAPSHOT OF` of the database after initialization. Executes `RESTORE DATABASE FROM DATABASE SNAPSHOT` in between each test execution. Reset duration should be expected to be between 80ms and 150ms.
 - Transactions (premium feature)
 Starts a transaction savepoint after initialization. Rolls back to the initialization savepoint between each test execution. Reset duration should be expected to be between 0ms and 3ms.


**Creation strategies**

 - TypeConfigurations (default)
Calls `EnsureDatabaseCreatedAsync` on the `DbContext` to create the database
 - Migrations
Calls `MigrateAsync` on the `DbContext` to create the database

#### PostgreSql

**Package** [Accelergreat.EntityFramework.PostgreSql](https://www.nuget.org/packages/Accelergreat.EntityFramework.PostgreSql)

**Class** [`PostgreSqlEntityFrameworkDatabaseComponent`](xref:Accelergreat.EntityFramework.PostgreSql.PostgreSqlEntityFrameworkDatabaseComponent`1)

**Configuration schema** see the `PostgreSqlEntityFramework` section in [schema.json](https://cdn.accelergreat.net/configuration/~(version)~/schema.json)
  
**Reset strategies**

 - DumpRestore (default)
 Calls `pg_dump` of the database after initialization. Calls `pg_restore` in between each test execution.
 - Transactions (premium feature)
 Starts a transaction and creates savepoint after initialization. Rolls back to the initialization savepoint between each test execution.

**Creation strategies**

 - TypeConfigurations
Calls EnsureDatabaseCreatedAsync on the DbContext to create the database
 - Migrations
Calls MigrateAsync on the DbContext to create the database

#### Sqlite

**Package** [Accelergreat.EntityFramework.Sqlite](https://www.nuget.org/packages/Accelergreat.EntityFramework.Sqlite)

**Class** [`SqliteEntityFrameworkDatabaseComponent`](xref:Accelergreat.EntityFramework.Sqlite.SqliteEntityFrameworkDatabaseComponent`1)

#### InMemory

**Package** [Accelergreat.EntityFramework.InMemory](https://www.nuget.org/packages/Accelergreat.EntityFramework.InMemory)

**Class** [`InMemoryEntityFrameworkDatabaseComponent`](xref:Accelergreat.EntityFramework.InMemory.InMemoryEntityFrameworkDatabaseComponent`1)

### GlobalData

You may hit a scenario where you want to have some global data setup at the beginning of your tests that persists throughout the entire test assembly execution.

All EntityFramework database components have a `OnDatabaseInitializedAsync` method that you can override to add such data that is saved before the initial state of the database is set.

Here is an example:

```csharp
namespace ExampleTestProject;

public class TestSqlServerDatabaseComponent : SqlServerEntityFrameworkDatabaseComponent<AdventureWorks2016Context>
{
    public TestSqlServerDatabaseComponent ()
    {
        AutoRegisterGlobalDataItemsInDbContextCreation = true;
    }

    protected override async Task OnDatabaseInitializedAsync(AdventureWorks2016Context context)
    {
        await context.AddAsync(new Person());

        await context.SaveChangesAsync();
    }
}
```

### Transactions (premium feature)
If you are using the transactions configuration for your database component, any dependant components / tests will need to connect to the database under the same transaction/connection.

Accelergreat has some extension methods to support this:

- [`ServiceCollectionExtensions.AddSqlServerDbContextUsingTransaction`](xref:Accelergreat.EntityFramework.SqlServer.Transactions.Extensions.ServiceCollectionExtensions.AddSqlServerDbContextUsingTransaction``1(Microsoft.Extensions.DependencyInjection.IServiceCollection,Accelergreat.Environments.IReadOnlyAccelergreatEnvironmentPipelineData,System.Nullable{Action{Microsoft.EntityFrameworkCore.Infrastructure.SqlServerDbContextOptionsBuilder}},System.Boolean))

    **For SQL Server only**. Removes service descriptors that EntityFramework adds when calling `AddDbContext` or `AddDbContextFactory` and re-registers using `AddDbContextFactory` to use the same transaction/connection.
- [`ServiceCollectionExtensions.AddPostgreSqlDbContextUsingTransaction`](xref:Accelergreat.EntityFramework.PostgreSql.Transaction.Extensions.ServiceCollectionExtensions.AddPostgreSqlDbContextUsingTransaction``1(Microsoft.Extensions.DependencyInjection.IServiceCollection,Accelergreat.Environments.IReadOnlyAccelergreatEnvironmentPipelineData,System.Nullable{Action{Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.NpgsqlDbContextOptionsBuilder}},System.Boolean))

    **For PostgreSQL only**. Removes service descriptors that EntityFramework adds when calling `AddDbContext` or `AddDbContextFactory` and re-registers using `AddDbContextFactory` to use the same transaction/connection.
- [`ServiceCollectionExtensions.AddAccelergreatDbContext`](xref:Accelergreat.EntityFramework.Extensions.ServiceCollectionExtensions.AddAccelergreatDbContext``1(Microsoft.Extensions.DependencyInjection.IServiceCollection,Accelergreat.Environments.IReadOnlyAccelergreatEnvironmentPipelineData,System.Boolean,Microsoft.Extensions.DependencyInjection.ServiceLifetime))

    Universal extension that supports all database components regardless if they are configured to use transactions or not. This uses the Accelergreat DbContextFactory to create the DbContext.

For use in a [`WebAppComponent`](xref:Accelergreat.Web.WebAppComponent`1) or [`KestrelWebAppComponent`](xref:Accelergreat.Web.KestrelWebAppComponent`1) you can call these extension methods when overriding the [`ConfigureWebHost`](xref:Accelergreat.Web.WebAppComponent`1.ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder)) method.

#### Transaction overriding

Transaction overriding is a an opt-in feature that is designed to support a scenario where you want to use the transactions reset strategy but your application you are testing creates transactions of its own. Which EntityFramework does not allow the creation of nested transactions.

Accelergreat has implemented overrides for EntityFramework's transaction classes. This effectively re-routes the following DbContext transaction methods:

| original method | destination method |
|--|--|
|Commit|CreateSavepoint|
|CommitAsync|CreateSavepointAsync|
|Rollback|RollbackToSavepoint|
|RollbackAsync|RollbackToSavepointAsync|

If this scenario suits your needs and you would like to have your cake and also be able to eat it, transaction overriding is for you.

To enable, `useTransactionOverriding` is an optional parameter that can be set to true in any of the Accelergreat extension methods around adding DbContexts specified above. 

### Web applications

  Both [`WebAppComponent`](xref:Accelergreat.Web.WebAppComponent`1) and [`KestrelWebAppComponent`](xref:Accelergreat.Web.KestrelWebAppComponent`1) extend `WebApplicationFactory`. You will be able to use these just like a normal WebApplicationFactory but with added Accelergreat magic and the ability to easily use as a component.

#### Web application (in memory)

**Package** [Accelergreat.Web](https://www.nuget.org/packages/Accelergreat.Web)

**Class** [`WebAppComponent`](xref:Accelergreat.Web.WebAppComponent`1)

#### Web application (kestrel)

**Package** [Accelergreat.Web](https://www.nuget.org/packages/Accelergreat.Web)

**Class** [`KestrelWebAppComponent`](xref:Accelergreat.Web.KestrelWebAppComponent`1)

## Build your own components

You will most likely hit a scenario when you want to build your own component to support functionality that Accelergreat hasn't yet covered. For example if you need to run a docker container to run the test database on.

### Template

All you need to do will need to create a class that implements the [`IAccelergreatComponent`](xref:Accelergreat.Components.IAccelergreatComponent) interface.

```csharp
namespace  ExampleTestProject;

public class ExampleComponent : IAccelergreatComponent
{
    async Task IAccelergreatComponent.InitializeAsync(IAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData)
    {
        /*
        * Initialize your component here.
        * This method will be called before the tests within the test assembly have started executing.
        * You can use accelergreatEnvironmentPipelineData to reference any data that a component that was initialized before this one has added.
        * You can add data to accelergreatEnvironmentPipelineData for use in any dependent components that initialize after this one.
        */
    }

    async Task IAccelergreatComponent.ResetAsync()
    {
        /*
        * Reset your component here.
        * This method is called between each test to enable you to put the component back to a state of your choosing.
        * This method will not be called if this component is registered as a singleton component.
        */
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        /*
        * Dispose your component here.
        * This method is called after all the test assembly has finished executing.
    */
    }
}
```