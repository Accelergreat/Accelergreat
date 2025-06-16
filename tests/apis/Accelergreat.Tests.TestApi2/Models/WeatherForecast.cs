using System.Text.Json.Serialization;

namespace Accelergreat.Tests.TestApi2.Models;

public class WeatherForecast
{
    [JsonPropertyName("summary")]
    public string Summary { get; set; } = default!;
}