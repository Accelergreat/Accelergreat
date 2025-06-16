using System;
using System.Threading.Tasks;
using Accelergreat.Environments.Pooling;
using Accelergreat.Tests.Api.Components;
using Accelergreat.Tests.EntityFramework.Relational.AdventureWorks.Models;
using Accelergreat.Tests.TestApi.Models;
using Accelergreat.Xunit;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace Accelergreat.Tests.Api.Tests;

public class WeatherForecastGetTests4 : AccelergreatXunitTest
{
    public WeatherForecastGetTests4(IAccelergreatEnvironmentPool environmentPool) : base(environmentPool)
    {
    }

    [Fact]
    public async Task WeatherForecast_Get_ReturnsValidSummaries()
    {
        // Arrange
        var testItem = new Currency
        {
            CurrencyCode = "GBP",
            Name = "British Pount",
            ModifiedDate = DateTime.Now
        };

        var testSqlServerDatabaseComponent = GetComponent<TestSqlServerDatabaseComponent>();

        var dbContextFactory = testSqlServerDatabaseComponent.DbContextFactory;

        await using (var context = dbContextFactory.NewDbContext())
        {
            await context.Set<Currency>().AddAsync(testItem);

            await context.SaveChangesAsync();
        }

        var possibleSummaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        var testApiWebComponent = GetComponent<TestApiWebAppComponent>();

        // Act
        var response = await testApiWebComponent.CreateClient().GetAsync("WeatherForecast");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var body = await response.Content.ReadAsStringAsync();

        var forecasts = JsonConvert.DeserializeObject<WeatherForecast[]>(body)!;

        foreach (var forecast in forecasts)
        {
            possibleSummaries.Should().Contain(forecast.Summary);
        }
    }

    [Fact]
    public async Task WeatherForecast_Get_ReturnsValidSummaries2()
    {
        // Arrange
        var testItem = new Currency
        {
            CurrencyCode = "GBP",
            Name = "British Pount",
            ModifiedDate = DateTime.Now
        };

        var testSqlServerDatabaseComponent = GetComponent<TestSqlServerDatabaseComponent>();

        var dbContextFactory = testSqlServerDatabaseComponent.DbContextFactory;

        await using (var context = dbContextFactory.NewDbContext())
        {
            await context.Set<Currency>().AddAsync(testItem);

            await context.SaveChangesAsync();
        }

        var possibleSummaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        var testApiWebComponent = GetComponent<TestApiWebAppComponent>();

        // Act
        var response = await testApiWebComponent.CreateClient().GetAsync("WeatherForecast");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var body = await response.Content.ReadAsStringAsync();

        var forecasts = JsonConvert.DeserializeObject<WeatherForecast[]>(body)!;

        foreach (var forecast in forecasts)
        {
            possibleSummaries.Should().Contain(forecast.Summary);
        }
    }

    [Fact]
    public async Task WeatherForecast_Get_ReturnsValidSummaries3()
    {
        // Arrange
        var testItem = new Currency
        {
            CurrencyCode = "GBP",
            Name = "British Pount",
            ModifiedDate = DateTime.Now
        };

        var testSqlServerDatabaseComponent = GetComponent<TestSqlServerDatabaseComponent>();

        var dbContextFactory = testSqlServerDatabaseComponent.DbContextFactory;

        await using (var context = dbContextFactory.NewDbContext())
        {
            await context.Set<Currency>().AddAsync(testItem);

            await context.SaveChangesAsync();
        }

        var possibleSummaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        var testApiWebComponent = GetComponent<TestApiWebAppComponent>();

        // Act
        var response = await testApiWebComponent.CreateClient().GetAsync("WeatherForecast");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var body = await response.Content.ReadAsStringAsync();

        var forecasts = JsonConvert.DeserializeObject<WeatherForecast[]>(body)!;

        foreach (var forecast in forecasts)
        {
            possibleSummaries.Should().Contain(forecast.Summary);
        }
    }
}