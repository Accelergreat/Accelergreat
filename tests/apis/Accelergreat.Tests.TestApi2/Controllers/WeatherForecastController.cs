using System.Text.Json;
using Accelergreat.Tests.TestApi2.Models;
using Microsoft.AspNetCore.Mvc;

namespace Accelergreat.Tests.TestApi2.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public WeatherForecastController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpGet]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        var response = await _httpClient.GetAsync("WeatherForecast");

        response.EnsureSuccessStatusCode();

        var responseText = await response.Content.ReadAsStringAsync();

        var weatherForecasts = JsonSerializer.Deserialize<WeatherForecast[]>(responseText);

        return weatherForecasts!;
    }

    [HttpGet("2")]
    public async Task<IEnumerable<WeatherForecast>> Get2()
    {
        using var brandNewHttpClient = new HttpClient();

        brandNewHttpClient.BaseAddress = _httpClient.BaseAddress;

        var response = await brandNewHttpClient.GetAsync("WeatherForecast");

        response.EnsureSuccessStatusCode();

        var responseText = await response.Content.ReadAsStringAsync();

        var weatherForecasts = JsonSerializer.Deserialize<WeatherForecast[]>(responseText);

        return weatherForecasts!;
    }
}