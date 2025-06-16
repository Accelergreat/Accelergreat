using System;
using System.Threading.Tasks;
using Accelergreat.Environments.Pooling;
using Accelergreat.Tests.EntityFramework.Relational.AdventureWorks.Models;
using Accelergreat.Tests.EntityFramework.SqLite.Components;
using Accelergreat.Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Accelergreat.Tests.EntityFramework.SqLite.Tests;

public class SqliteSqlDatabaseTests : AccelergreatXunitTest
{
    public SqliteSqlDatabaseTests(IAccelergreatEnvironmentPool environmentPool) : base(environmentPool)
    {
    }

    [Fact]
    public Task CanUseDatabase1()
    {
        return RunTest();
    }

    private async Task RunTest()
    {
        var testItem = new Currency
        {
            CurrencyCode = "GBP",
            Name = "British Pount",
            ModifiedDate = DateTime.UtcNow
        };

        var testSqliteSqlDatabaseComponent = GetComponent<TestSqliteSqlDatabaseComponent>();

        var dbContextFactory = testSqliteSqlDatabaseComponent.DbContextFactory;

        await using (var context = dbContextFactory.NewDbContext())
        {
            await context.Set<Currency>().AddAsync(testItem);

            await context.SaveChangesAsync();
        }

        await using (var context = dbContextFactory.NewDbContext())
        {
            var savedTestItem = await context.Set<Currency>().SingleAsync();

            savedTestItem.CurrencyCode.Should().Be(testItem.CurrencyCode);
        }
    }
}