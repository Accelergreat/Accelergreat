using System;
using Accelergreat.Environments;
using Accelergreat.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Accelergreat.Tests.MinimalApiTests.Components;

internal class MinimalApiWebAppComponent : KestrelWebAppComponent<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder, IConfiguration configuration,
        IReadOnlyAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData)
    {
        builder.UseContentRoot(AppDomain.CurrentDomain.BaseDirectory);

        builder.UseEnvironment("IntegrationTesting");
    }
}