using System;
using Accelergreat.EntityFramework.Extensions;
using Accelergreat.Environments;
using Accelergreat.Tests.EntityFramework.Relational.AdventureWorks;
using Accelergreat.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Accelergreat.Tests.Api.PostgreSql.Components;

public class TestApiWebAppComponent : KestrelWebAppComponent<TestApi.Postgres.Startup>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder, IConfiguration configuration,
        IReadOnlyAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData)
    {
        builder.UseContentRoot(AppDomain.CurrentDomain.BaseDirectory);

        builder.UseEnvironment("IntegrationTesting");

        builder.ConfigureServices(services =>
        {
            services.AddAccelergreatDbContext<AdventureWorks2016Context>(
                accelergreatEnvironmentPipelineData, useTransactionOverriding: true);

            services.AddAccelergreatProxiedDbContext<AdventureWorks2016Context, AdventureWorks2016Context2>(
                accelergreatEnvironmentPipelineData, useTransactionOverriding: true);
        });
    }
}