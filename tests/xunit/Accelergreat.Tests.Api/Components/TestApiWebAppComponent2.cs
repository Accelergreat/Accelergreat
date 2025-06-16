using System;
using Accelergreat.Environments;
using Accelergreat.Web;
using Accelergreat.Web.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Accelergreat.Tests.Api.Components;

public class TestApiWebAppComponent2 : WebAppComponent<TestApi2.Startup>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder, IConfiguration configuration,
        IReadOnlyAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData)
    {
        builder.UseContentRoot(AppDomain.CurrentDomain.BaseDirectory);

        builder.UseEnvironment("IntegrationTesting");

        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton(_ => accelergreatEnvironmentPipelineData.GetWebAppHttpClient<TestApi.Startup>());
        });
    }
}