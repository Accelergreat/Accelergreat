using System;
using System.Threading.Tasks;
using Accelergreat.Environments.Pooling;
using Accelergreat.Tests.EntityFramework.PostgreSql.Components;
using Accelergreat.Tests.EntityFramework.Relational.AdventureWorks.Models;
using Accelergreat.Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Accelergreat.Tests.EntityFramework.PostgreSql.Tests;

public class PostgreSqlDatabaseTests3 : AccelergreatXunitTest
{
    public PostgreSqlDatabaseTests3(IAccelergreatEnvironmentPool environmentPool) : base(environmentPool)
    {
    }

    [Fact]
    public Task CanUseDatabase1()
    {
        return RunTest();
    }

    [Fact]
    public Task CanUseDatabase2()
    {
        return RunTest();
    }

    [Fact]
    public Task CanUseDatabase3()
    {
        return RunTest();
    }

    [Fact]
    public Task CanUseDatabase4()
    {
        return RunTest();
    }

    [Fact]
    public Task CanUseDatabase5()
    {
        return RunTest();
    }

    [Fact]
    public Task CanUseDatabase6()
    {
        return RunTest();
    }

    [Fact]
    public Task CanUseDatabase7()
    {
        return RunTest();
    }

    [Fact]
    public Task CanUseDatabase8()
    {
        return RunTest();
    }

    [Fact]
    public Task CanUseDatabase9()
    {
        return RunTest();
    }

    [Fact]
    public Task CanUseDatabase10()
    {
        return RunTest();
    }

    [Fact]
    public Task CanUseDatabase11()
    {
        return RunTest();
    }

    [Fact]
    public Task CanUseDatabase12()
    {
        return RunTest();
    }

    [Fact]
    public Task CanUseDatabase13()
    {
        return RunTest();
    }

    [Fact]
    public Task CanUseDatabase14()
    {
        return RunTest();
    }

    [Fact]
    public Task CanUseDatabase15()
    {
        return RunTest();
    }

    [Fact]
    public Task CanUseDatabase16()
    {
        return RunTest();
    }

    [Fact]
    public Task CanUseDatabase17()
    {
        return RunTest();
    }

    [Fact]
    public Task CanUseDatabase18()
    {
        return RunTest();
    }

    [Fact]
    public Task CanUseDatabase19()
    {
        return RunTest();
    }

    [Fact]
    public Task CanUseDatabase20()
    {
        return RunTest();
    }

    [Fact]
    public Task CanUseDatabase21()
    {
        return RunTest();
    }

    [Fact]
    public Task CanUseDatabase22()
    {
        return RunTest();
    }

    [Fact]
    public Task CanUseDatabase23()
    {
        return RunTest();
    }

    [Fact]
    public Task CanUseDatabase24()
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

        var testSqlServerDatabaseComponent = GetComponent<TestPostgreSqlDatabaseComponent>();

        var dbContextFactory = testSqlServerDatabaseComponent.DbContextFactory;

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