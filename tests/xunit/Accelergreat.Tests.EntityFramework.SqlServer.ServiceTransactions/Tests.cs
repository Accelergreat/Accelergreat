using System.Threading.Tasks;
using Accelergreat.Environments.Pooling;
using Accelergreat.Tests.EntityFramework.Relational.AdventureWorks.Models;
using Accelergreat.Tests.EntityFramework.SqlServer.ServiceTransactions.Components;
using Accelergreat.Xunit;
using FluentAssertions;
using Xunit;

namespace Accelergreat.Tests.EntityFramework.SqlServer.ServiceTransactions;

public class Tests : AccelergreatXunitTest
{
    public Tests(IAccelergreatEnvironmentPool environmentPool) : base(environmentPool)
    {
    }

    [Fact]
    internal async Task TheServiceUsesTransactionOverriding()
    {
        var databaseComponent = GetComponent<TestSqlServerDatabaseComponent>();

        await using (var transactionOverridingContext = databaseComponent.DbContextFactory.NewDbContext(true))
        {
            var sut = new TestService(transactionOverridingContext);

            await sut.InsertCurrencyViaTransactionAsync();
        }

        await using (var normalContext = databaseComponent.DbContextFactory.NewDbContext())
        {
            normalContext.Set<Currency>().Should().ContainSingle();
        }
    }

    [Fact]
    internal async Task WhenCallingSubsequentTransactionsThenItIsFine()
    {
        var databaseComponent = GetComponent<TestSqlServerDatabaseComponent>();

        await using (var transactionOverridingContext = databaseComponent.DbContextFactory.NewDbContext(true))
        {
            var sut = new TestService(transactionOverridingContext);

            await sut.InsertCurrencyViaTransactionAsync("GBP", "British Pound");
            await sut.InsertCurrencyViaTransactionAsync("USD", "United States Dollar");
        }

        await using (var normalContext = databaseComponent.DbContextFactory.NewDbContext())
        {
            normalContext.Set<Currency>().Should().HaveCount(2);
        }
    }
}