using System;
using System.Threading.Tasks;
using Accelergreat.EntityFramework.SqlServer;
using Accelergreat.Tests.EntityFramework.Relational.AdventureWorks;
using Accelergreat.Tests.EntityFramework.Relational.AdventureWorks.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace Accelergreat.Tests.EntityFramework.SqlServer.Components;

public class TestSqlServerDatabaseComponent : SqlServerEntityFrameworkDatabaseComponent<AdventureWorks2016Context>
{
    public TestSqlServerDatabaseComponent(IConfiguration configuration) : base(configuration)
    {
        AutoRegisterGlobalDataItemsInDbContextCreation = true;
    }

    protected override void ConfigureDbContextOptions(SqlServerDbContextOptionsBuilder options)
    {
        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
    }

    protected override async Task OnDatabaseInitializedAsync(AdventureWorks2016Context context)
    {
        var testItem = new Currency
        {
            CurrencyCode = "EUR",
            Name = "Euro",
            ModifiedDate = DateTime.Now
        };

        context.Add(testItem);

        await context.SaveChangesAsync();

        await base.OnDatabaseInitializedAsync(context);
    }
}