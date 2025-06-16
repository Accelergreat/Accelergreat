using System.Net;
using System.Threading.Tasks;
using Accelergreat.Environments.Pooling;
using Accelergreat.Tests.Api.Components;
using Accelergreat.Tests.EntityFramework.Relational.AdventureWorks;
using Accelergreat.Tests.EntityFramework.Relational.AdventureWorks.Models;
using Accelergreat.Xunit;
using FluentAssertions;
using Xunit;

namespace Accelergreat.Tests.Api.Tests;

public class TransactionTests : AccelergreatXunitTest
{
    public TransactionTests(IAccelergreatEnvironmentPool environmentPool) : base(environmentPool)
    {
    }

    [Fact]
    public async Task Add_Standard()
    {
        var testApiWebComponent = GetComponent<TestApiWebAppComponent>();

        // Act
        var response = await testApiWebComponent.CreateClient().GetAsync("WeatherForecast/addstandard");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var testSqlServerDatabaseComponent = GetComponent<TestSqlServerDatabaseComponent>();

        var dbContextFactory = testSqlServerDatabaseComponent.DbContextFactory;

        await using var context = dbContextFactory.NewDbContext();
        context.Set<Currency>().Should().ContainSingle();
    }

    [Fact]
    public async Task Add_Standard_Via_Proxied_Context()
    {
        var testApiWebComponent = GetComponent<TestApiWebAppComponent>();

        // Act
        var response = await testApiWebComponent.CreateClient().GetAsync("WeatherForecast/addstandardviaproxiedcontext");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var testSqlServerDatabaseComponent = GetComponent<TestSqlServerDatabaseComponent>();

        var dbContextFactory = testSqlServerDatabaseComponent.DbContextFactory.ChangeContextType<AdventureWorks2016Context2>();

        await using var context = dbContextFactory.NewDbContext();
        context.Set<Currency>().Should().ContainSingle();
    }

    [Fact]
    public async Task Add_Via_Transaction_And_Commit()
    {
        var testApiWebComponent = GetComponent<TestApiWebAppComponent>();

        // Act
        var response = await testApiWebComponent.CreateClient().GetAsync("WeatherForecast/addviatransactionandcommit");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var testSqlServerDatabaseComponent = GetComponent<TestSqlServerDatabaseComponent>();

        var dbContextFactory = testSqlServerDatabaseComponent.DbContextFactory;

        await using var context = dbContextFactory.NewDbContext();
        context.Set<Currency>().Should().ContainSingle();
    }

    [Fact]
    public async Task Add_Via_Transaction_And_Rollback()
    {
        var testApiWebComponent = GetComponent<TestApiWebAppComponent>();

        // Act
        var response = await testApiWebComponent.CreateClient().GetAsync("WeatherForecast/addviatransactionandrollback");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var testSqlServerDatabaseComponent = GetComponent<TestSqlServerDatabaseComponent>();

        var dbContextFactory = testSqlServerDatabaseComponent.DbContextFactory;

        await using var context = dbContextFactory.NewDbContext();
        context.Set<Currency>().Should().BeEmpty();
    }
    [Fact]
    public async Task Query()
    {
        var testApiWebComponent = GetComponent<TestApiWebAppComponent>();

        // Act
        var response = await testApiWebComponent.CreateClient().GetAsync("WeatherForecast/query");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}