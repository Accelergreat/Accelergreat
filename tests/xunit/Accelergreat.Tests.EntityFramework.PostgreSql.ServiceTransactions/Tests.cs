// Copyright (c) Nanogunn Ltd. All rights reserved.
// Licensed under https://cdn.accelergreat.net/legal/END_USER_LICENSE_AGREEMENT_Accelergreat.txt.

using System.Threading.Tasks;
using Accelergreat.Environments.Pooling;
using Accelergreat.Tests.EntityFramework.PostgreSql.ServiceTransactions.Components;
using Accelergreat.Tests.EntityFramework.Relational.AdventureWorks.Models;
using Accelergreat.Xunit;
using FluentAssertions;
using Xunit;

namespace Accelergreat.Tests.EntityFramework.PostgreSql.ServiceTransactions;

public class Tests : AccelergreatXunitTest
{
    public Tests(IAccelergreatEnvironmentPool environmentPool) : base(environmentPool)
    {
    }

    [Fact]
    internal async Task TheServiceUsesTransactionOverriding()
    {
        var databaseComponent = GetComponent<TestPostgreSqlDatabaseComponent>();

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
        var databaseComponent = GetComponent<TestPostgreSqlDatabaseComponent>();

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