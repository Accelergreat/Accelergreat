using System.Threading.Tasks;
using Accelergreat.Environments.Pooling;
using Accelergreat.Tests.Api.Components;
using Accelergreat.Tests.TestApi.Models;
using Accelergreat.Xunit;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace Accelergreat.Tests.Api.Tests;

public class OneApiCallsTheOtherApiTests : AccelergreatXunitTest
{
    public OneApiCallsTheOtherApiTests(IAccelergreatEnvironmentPool environmentPool) : base(environmentPool)
    {
    }

    [Fact]
    public async Task FistApi_Calls_The_SecondApi_Via_HttpClient()
    {
        // Arrange
        var possibleSummaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        var testApiWebComponent2 = GetComponent<TestApiWebAppComponent2>();

        // Act
        var response = await testApiWebComponent2.CreateClient().GetAsync("WeatherForecast");

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
    public async Task FistApi_Calls_The_SecondApi_Via_Url()
    {
        // Arrange
        var possibleSummaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        var testApiWebComponent2 = GetComponent<TestApiWebAppComponent2>();

        // Act
        var response = await testApiWebComponent2.CreateClient().GetAsync("WeatherForecast/2");

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