using System;
using System.Threading.Tasks;
using Accelergreat.Tests.EntityFramework.Relational.AdventureWorks;
using Accelergreat.Tests.EntityFramework.Relational.AdventureWorks.Models;
using Microsoft.EntityFrameworkCore;
#nullable enable

namespace Accelergreat.Tests.EntityFramework.SqlServer.ServiceTransactions;

public class TestService
{
    private readonly AdventureWorks2016Context _context;

    public TestService(AdventureWorks2016Context context)
    {
        _context = context;
    }

    public Task InsertCurrencyViaTransactionAsync(string? currencyCode = null, string? currencyName = null)
    {
        return _context.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var testItem = new Currency
            {
                CurrencyCode = currencyCode ?? "GBP",
                Name = currencyName ?? "British Pount",
                ModifiedDate = DateTime.Now
            };

            _context.Currencies.Add(testItem);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        });
    }
}