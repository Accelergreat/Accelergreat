---
uid: DeveloperDocumentation.Index
---

# Ultra Fast Integration Tests

**Accelergreat your tests.**

Write and execute efficient and high-performance integration tests with ease. Accelergreat is a powerful .NET integration testing solution that automatically provisions and manages external dependencies for your tests, such as databases and APIs. Simplify your integration testing development process today.

## Overview

Accelergreat is a new and innovative testing platform designed to make integration testing easier.

Integration tests are essential to ensuring your code works and stays working. However, they have been challenging to write and run quickly in the past, earning a reputation as a problem area in the development world.

With Accelergreat we intend to make this a problem of the past by doing all the hard work for you.

Accelergreat currently supports [xUnit](https://xunit.net/) and plans to support other test runners in the future.

## NuGet Packages

[![nuget v~(version)~](https://img.shields.io/badge/nuget-v~(version)~-blue)](https://www.nuget.org/profiles/Nanogunn) ![net6.0 | net7.0](https://img.shields.io/badge/dotnet-net6.0%20%7C%20net7.0%20%7C%20net8.0-red)

Accelergreat has a range of nuget packages for different feature sets:

### Core packages

| Package |   |
| ------- | - |
| [`Accelergreat`](https://www.nuget.org/packages/Accelergreat) | Referenced by all other Accelergreat packages. You can also reference this package to make your own custom Accelergreat components. |
| [`Accelergreat.EntityFramework`](https://www.nuget.org/packages/Accelergreat.EntityFramework) | Core Entity Framework package referenced by Accelergreat's Entity Framework packages. |

### Test framework packages

The test framework packages contain Accelergreat's test framework specific code. All other packages can be used interchangeably with each test framework package. 

| Framework | Package |
| --------- | ------- |
| [xUnit](https://xunit.net/) | [`Accelergreat.Xunit`](https://www.nuget.org/packages/Accelergreat.Xunit) |

### Application packages

Accelergreat can run instances of your application. 

| Application type | Package |
| ---------------- | ------- |
| Web API | [`Accelergreat.Web`](https://www.nuget.org/packages/Accelergreat.Web) |

### Entity Framework packages

Accelergreat supports different database providers with the following entity framework packages. Accelergreat fully manages databse creation, reset between tests and cleanup at the end of the test run.

| Provider | Package |
| :-------- | ------- |
| Sql Server | [`Accelergreat.EntityFramework.SqlServer`](https://www.nuget.org/packages/Accelergreat.EntityFramework.SqlServer) |
| Postgre SQL | [`Accelergreat.EntityFramework.PostgreSql`](https://www.nuget.org/packages/Accelergreat.EntityFramework.PostgreSql) |
| Sqlite | [`Accelergreat.EntityFramework.Sqlite`](https://www.nuget.org/packages/Accelergreat.EntityFramework.Sqlite) |
| In Memory | [`Accelergreat.EntityFramework.InMemory`](https://www.nuget.org/packages/Accelergreat.EntityFramework.InMemory) |

### Future packages

Accelergreat is growing and open to requests. See [Community & Support](#community--support) for how to get in touch.

### Version Compatibility

Accelergreat follows [Semantic Versioning 2.0.0](https://semver.org/spec/v2.0.0.html) for releases.

Accelergreat keeps up-to date with dotnet releases and each major version will support all currently in-support versions of dotnet at the time of release.

Accelergreat will not introduce amy breaking public API changes for minor and patch versions.

# Getting started (xUnit)

Getting startes is easy, whether you are starting a new test project, or want to migrate an existing one.

## High level steps

1. **Install** [`Accelergreat.Xunit`](https://www.nuget.org/packages/Accelergreat.Xunit) in your test project.

    ```shell
    dotnet add package Accelergreat.Xunit
    ```

2. **Define** components needed.

    One of the core features of Accelergreat is the concept of components. A component represents a dependency for your tests. For example a database or a web API.

    Accelergreat offers a range of pre-built components that can be extended for your specific needs. Or, you can build your own components by implementing the `IAccelergreatComponent` interface.

    To further understand components beyond what the getting started guide explains, the different components Accelergreat offers and to understand how to build your own components. See the dedicated guide on [components](components.md).

3. **Configure** the environment

    Configure the environment by registering components in a startup class that implements the `IAccelergreatStartup` interface. Just like how you setup dependency injection for your application, Accelergreat uses dependency injection for environment and component resolution for your tests.

4. Write your tests! 🚀


## Detailed steps (example use case)
Accelergreat has been designed with modularity and flexibility in mind. Whilst the example we have provided below may not match your specific use case, this example was chosen to best show the capabilities of Accelergreat. It should be an effective reference point for your own implementations.

**Use case example:** Database (Entity Framework SQL Server) + Web API integration test

**To see other examples**, you can look on our [GitHub examples repo](https://github.com/Accelergreat/Examples).

#### 1. Install the required packages

- [`Accelergreat.EntityFramework.SqlServer`](https://www.nuget.org/packages/Accelergreat.EntityFramework.SqlServer)
    ```shell
    dotnet add package Accelergreat.EntityFramework.SqlServer
    ```
- [`Accelergreat.Web`](https://www.nuget.org/packages/Accelergreat.Web)
    ```shell
    dotnet add package Accelergreat.Web
    ```

#### 2. Create the Accelergreat Components
In our example we only need two components. One for our SQL Server database, and one for our Web API.

##### **Database component**
Create a class that inherits from [`SqlServerEntityFrameworkDatabaseComponent`](xref:Accelergreat.EntityFramework.SqlServer.SqlServerEntityFrameworkDatabaseComponent`1).

If you need to override any of the configuration values, see [`SqlServerEntityFrameworkConfiguration`](xref:Accelergreat.EntityFramework.SqlServer.SqlServerEntityFrameworkConfiguration) for the defaults.

``` C#
using Accelergreat.EntityFramework.SqlServer;
using Microsoft.Extensions.Configuration;

namespace ExampleTestProject.Components;

public class ExampleDatabaseComponent : SqlServerEntityFrameworkDatabaseComponent<ExampleDbContext>
{
    public ExampleDatabaseComponent(IConfiguration configuration) : base(configuration)
    {
    }
}
```

##### **Web API component**
To interact with a .NET Web API in our integration tests, we need to create a Web App Component.

Web app components run an instance of the API defined by the Web API startup class passed into the generic type parameter. The following steps will set up a component for you:

1. Create a class derived from [`WebAppComponent`](xref:Accelergreat.Web.WebAppComponent`1).
2. Inject the database connection string(s).
3. [Optional] overriding appsettings.

Create the class derived from [`WebAppComponent`](xref:Accelergreat.Web.WebAppComponent`1)
 
To create a Web App Component, create a class that inherits from [`WebAppComponent`](xref:Accelergreat.Web.WebAppComponent`1) and pass in your Web API startup class as the generic type parameter.

**Using an API startup class**
``` C#
using System.Collections.Generic;
using Accelergreat.Web;
using Accelergreat.Web.Extensions;
using Microsoft.Extensions.Configuration;

namespace ExampleTestProject.Components;

public class ExampleApiComponent : WebAppComponent<ExampleApi.Startup>
{
    protected override void BuildConfiguration(
        IConfigurationBuilder configurationBuilder,
        IReadOnlyAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData)
    {
        configurationBuilder.AddEntityFrameworkDatabaseConnectionString<ExampleDbContext>(
            "ExampleConnectionStringName", accelergreatEnvironmentPipelineData);
    }
}
```
**Using an API program class**

With the recent introduction of using the program.cs file to replace the startup class, you can now configure your application entirely in Program.cs . The Progam class is now in the global
App Domain as an internal class of the Api assembly.  If you are using miniminal apis then you will have to expose `InternalsVisibleTo` to the Test Assembly by editing the csproj file of the Api project:

``` xml
<Project Sdk="Microsoft.NET.Sdk.Web">

...

	<ItemGroup>
		<InternalsVisibleTo Include="ExampleTestProject" />
	</ItemGroup>
...

```

``` C#
using System;
using System.Collections.Generic;
using Accelergreat.EntityFramework.Extensions;
using Accelergreat.Environments;
using Accelergreat.Web;
using GuestAndActivities.Data.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace ExampleTestProject.Components;

internal class ExampleApiComponent : WebAppComponent<Program>
{

    protected override void BuildConfiguration(
            IConfigurationBuilder configurationBuilder,
            IReadOnlyAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData)
    {
        configurationBuilder.AddEntityFrameworkDatabaseConnectionString<ExampleDbContext>(
            "ExampleConnectionStringName", accelergreatEnvironmentPipelineData);
    }
}
```


**Inject the database connection string(s)**

In the examples above, we need to provide the database connection string to the Web API configuration a database connection string. We do this by overriding > [`BuildConfiguration`](xref:Accelergreat.Web.WebAppComponent`1.BuildConfiguration(Microsoft.Extensions.Configuration.IConfigurationBuilder,Accelergreat.Environments.IReadOnlyAccelergreatEnvironmentPipelineData)) and calling [`configurationBuilder.AddEntityFrameworkDatabaseConnectionString`](xref:Accelergreat.EntityFramework.Extensions.ConfigurationBuilderExtensions.AddEntityFrameworkDatabaseConnectionString``1(Microsoft.Extensions.Configuration.IConfigurationBuilder,System.String,Accelergreat.Environments.IReadOnlyAccelergreatEnvironmentPipelineData)).


#### 3. Create a `Startup` class

Create a `Startup` class in your test project that implements [`IAccelergreatStartup`](xref:Accelergreat.Xunit.IAccelergreatStartup).

we need to register our components. In the `Configure` implementation, call [`builder.AddAccelergreatComponent`](xref:Accelergreat.Xunit.Extensions.ServiceCollectionExtensions.AddAccelergreatComponent``1(Microsoft.Extensions.DependencyInjection.IServiceCollection)) for each component as the type parameter.

**Important**
The order in which components are initialized is defined by the order they are registered.

In our example, we need to initialize the database before the web API so that it can access the database connection string.

``` C#
using Accelergreat.Xunit;
using ExampleTestProject.Components;

namespace ExampleTestProject;

public class Startup : IAccelergreatStartup
{
    public void Configure(IAccelergreatBuilder builder)
    {
        builder.AddAccelergreatComponent<ExampleDatabaseComponent>();
        builder.AddAccelergreatComponent<ExampleApiComponent>();
    }
}
```

#### 4. Write your first Accelegreat Integration Test

Now that we've got everything set up, we're going to write our first test.

For this example, we'll also be using [FluentAssertions](https://github.com/fluentassertions/fluentassertions) to keep our assertion code clean and easy to read. We'll also follow the Arrange Act Assert (AAA) pattern:

**Arrange** Set up and insert our test data

**Act** Call the API endpoint

**Assert** Validate that the response data is correct

Create a test that that inherits from [`AccelergreatXunitTest`](xref:Accelergreat.Xunit.AccelergreatXunitTest).

``` C#
using System.Threading.Tasks;
using Accelergreat.Environments.Pooling;
using Accelergreat.Xunit;
using ExampleTestProject.Components;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace ExampleTestProject.Tests;

public class ExampleTests : AccelergreatXunitTest
{
    public ExampleTests(IAccelergreatEnvironmentPool environmentPool) : base(environmentPool)
    {
    }

    [Fact]
    public async Task Examples_GetById_ReturnsCorrectExample()
    {
        // Arrange
        var exampleEntity = new ExampleEntity();

        var dbContextFactory = GetComponent<ExampleDatabaseComponent>().DbContextFactory;

        await using (var context = dbContextFactory.NewDbContext())
        {
            context.Set<ExampleEntity>().Add(exampleEntity);

            await context.SaveChangesAsync();
        }

        var httpClient = GetComponent<ExampleApiComponent>().CreateClient();

        // Act
        var httpResponseMessage = await httpClient.GetAsync($"examples/{exampleEntity.Id}");

        // Assert
        httpResponseMessage.IsSuccessStatusCode.Should().BeTrue();

        var body = await httpResponseMessage.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<ExampleEntity>(body)!;

        result.Id.Should().Be(exampleEntity.Id);
    }
}
```

## Extensibility



To further understand components beyond what the below getting started guide shows, read about all the different components Accelergreat offers and to understand how to build your own components. Please follow [this guide](components.md).


## Configuration

Accelergreat supports the overriding of configuration for managed external dependencies such as databases.

Accelergreat will read the following files in the root level of your test projects that have the `CopyToOutputDirectory` setting set to `Always` or `PreserveNewest`:

- `accelergreat.json`
- `accelergreat.{environment}.json`

`accelergreat.{environment}.json` is supported to allow you to have different configurations for your local machine and CI pipeline.

`{environment}` is defined by setting an environment variable called `ACCELERGREAT_ENVIRONMENT`.

A schema has been provided for the configuration files:

``` json
{
    "$schema": "https://cdn.accelergreat.net/configuration/~(version)~/schema.json#"
}
```
---

### Example

In the development environment, the developer wants to run their application in memory to be able to make use of Visual Studio debugging tools, and also make use of faster database reset speed by using the Transactions reset strategy.

- `accelergreat.development.json`
    ```json
    {
        "$schema": "https://cdn.accelergreat.net/configuration/~(version)~/schema.json#",
        "Infrastructure": { // Custom config
            "Setup": "InMemory"
        },
        "SqlServerEntityFramework": {
            "ResetStrategy": "Transactions"
        }
    }
    ```

In the CI environment, the developer wants to run their tests against the docker container that will be deployed to the hosted environments. In this case, the developer is using [IL-trimming](https://learn.microsoft.com/en-us/dotnet/core/deploying/trimming/trimming-option) in the dotnet publish step of their Dockerfile.

- `accelergreat.CI.json`
    ```json
    {
        "$schema": "https://cdn.accelergreat.net/configuration/~(version)~/schema.json#",
        "Infrastructure": { // Custom config
            "Setup": "Containers"
        },
        "SqlServerEntityFramework": {
            "ResetStrategy": "SnapshotRollback"
        }
    }
    ```

On their local machine, the developer would set the `ACCELERGREAT_ENVIRONMENT` environment variable with value `development`, in the CI environment, the value would be `CI`.


**To see other examples**, you can look on our [GitHub examples repo](https://github.com/Accelergreat/Examples).

## Parallel execution

One of the best features of Accelergreat is the ability to have your integration tests run in parallel, gaining massive performance improvements on multi-threaded machines.

Accelergreat will automatically execute in parallel when the following criteria is met:

- More than 1 test collection is queued for execution.
- [`parallelizeTestCollections`](https://xunit.net/docs/configuration-files#parallelizeTestCollections) has not been set to false in your [`xunit.runner.json`](https://xunit.net/docs/configuration-files).
- [`maxParallelThreads`](https://xunit.net/docs/configuration-files#maxParallelThreads) has not been set to 1 in your [`xunit.runner.json`](https://xunit.net/docs/configuration-files).

By default, Accelergreat uses the max threads allowed from the number of logical processors on the machine. You can override this by setting the [`maxParallelThreads`](https://xunit.net/docs/configuration-files#maxParallelThreads) property in your [`xunit.runner.json`](https://xunit.net/docs/configuration-files) configuration file.

## Database transactions

Resetting a databse between tests takes time. By default, Accelergreat uses the `SnapshotRollback` reset strategy that creates a snapshot of the database before the tests run, and performs a rollback on the database between each test.

The time it takes to reset a database will greatly impact how long it takes the entire test suite to run.

With the premium `Transactions` reset strategy, Accelergreat performs magic transaction management and all of your test database changes are made within a single transaction. 

If your application uses transactions that is covered by your tests, you can enable `TransactionOveriding` and Accelergreat will do even more magic behind the scenes to nest your transactions within Accelergreats transaction.

This paired with the Parallel execution feature will let your tests 'make the jump to light speed' - Han Solo.

# Licensing / Pricing
Accelergreat is free to use.

# Debugging Best Practices

**Important**
When debugging tests in Visual Studio, do not click the stop button. Even if an exception occurs, click continue and let the test run through. If you click the stop button xunit will not run cleanup operations and dependencies will not be disposed of. This is especially important when debugging tests that use database dependencies.

# Community & support

Email [mail@accelergreat.net](mailto:mail@accelergreat.net) for direct communication and support requests.

Join our [Discord channel](https://discord.com/channels/1175044305988091995/1175044307032481804).

See our [GitHub](https://github.com/Accelergreat/Examples).

See our [FAQs](https://accelergreat.net/faqs).