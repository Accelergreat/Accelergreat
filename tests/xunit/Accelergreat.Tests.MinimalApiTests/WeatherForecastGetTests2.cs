using System.Threading.Tasks;
using Accelergreat.Environments.Pooling;
using Accelergreat.Tests.MinimalApi;
using Accelergreat.Tests.MinimalApiTests.Components;
using Accelergreat.Xunit;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace Accelergreat.Tests.MinimalApiTests;

public class WeatherForecastGetTests2 : AccelergreatXunitTest
{
    public WeatherForecastGetTests2(IAccelergreatEnvironmentPool environmentPool) : base(environmentPool)
    {
    }

    [Fact]
    public async Task WeatherForecast_Get_ReturnsValidSummaries()
    {
        // Arrange
        var possibleSummaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        var testApiWebComponent = GetComponent<MinimalApiWebAppComponent>();

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