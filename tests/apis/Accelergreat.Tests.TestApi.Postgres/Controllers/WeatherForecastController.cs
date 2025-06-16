using Accelergreat.Tests.EntityFramework.Relational.AdventureWorks;
using Accelergreat.Tests.EntityFramework.Relational.AdventureWorks.Models;
using Accelergreat.Tests.TestApi.Postgres.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Accelergreat.Tests.TestApi.Postgres.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    [HttpGet("")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(_ => new WeatherForecast
            {
                Summary = Summaries[0]
            })
            .ToArray();
    }

    [HttpGet("addstandard")]
    public async Task<IActionResult> Add([FromServices] AdventureWorks2016Context context)
    {
        context.Currencies.Add(new Currency
        {
            CurrencyCode = "GBP",
            Name = "British Pount",
            ModifiedDate = DateTime.UtcNow
        });

        await context.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("addstandardviaproxiedcontext")]
    public async Task<IActionResult> Add([FromServices] AdventureWorks2016Context2 context)
    {
        context.Currencies.Add(new Currency
        {
            CurrencyCode = "GBP",
            Name = "British Pount",
            ModifiedDate = DateTime.UtcNow
        });

        await context.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("query")]
    public async Task<IActionResult> Query([FromServices] AdventureWorks2016Context context)
    {
        await context.Currencies.Where(x => x.Name == "b").Select(x => x.Name).ToListAsync();

        return Ok();
    }

    [HttpGet("addviatransactionandcommit")]
    public async Task<IActionResult> AddViaTransactionAndCommit([FromServices] AdventureWorks2016Context context)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        context.Currencies.Add(new Currency
        {
            CurrencyCode = "GBP",
            Name = "British Pount",
            ModifiedDate = DateTime.UtcNow
        });

        await context.SaveChangesAsync();

        await transaction.CommitAsync();

        return Ok();
    }

    [HttpGet("addviatransactionandrollback")]
    public async Task<IActionResult> AddViaTransactionAndRollback([FromServices] AdventureWorks2016Context context)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        context.Currencies.Add(new Currency
        {
            CurrencyCode = "GBP",
            Name = "British Pount",
            ModifiedDate = DateTime.UtcNow
        });

        await context.SaveChangesAsync();

        await transaction.RollbackAsync();

        return Ok();
    }
}