using Accelergreat.Components;
using Accelergreat.Environments;
using Accelergreat.Web.Extensions;
using Accelergreat.Web.Factories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Accelergreat.Web;

/// <summary>
/// A component for hosting a web application via <see cref="IWebHost"/>.
/// </summary>
/// <typeparam name="TEntryPoint">The Startup class of the web application to be hosted.</typeparam>
public class KestrelWebAppComponent<TEntryPoint> : KestrelWebApplicationFactory<TEntryPoint>, IAccelergreatComponent where TEntryPoint : class
{
    private Action<IWebHostBuilder>? _configureWebHostAction;

    protected sealed override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _configureWebHostAction?.Invoke(builder);

        base.ConfigureWebHost(builder);
    }

    /// <summary>
    /// Override this method to configure the <see cref="IWebHostBuilder"/>.
    /// <para>The <paramref name="configuration"/> is automatically applied with <see cref="HostingAbstractionsWebHostBuilderExtensions.UseConfiguration(IWebHostBuilder,IConfiguration)"/>.</para>
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <param name="accelergreatEnvironmentPipelineData">See <see cref="AccelergreatEnvironmentPipelineDataKeys"/> for pre-defined keys.</param>
    protected virtual void ConfigureWebHost(IWebHostBuilder builder, IConfiguration configuration, IReadOnlyAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData)
    {
    }

    /// <summary>
    /// Override this method to add configuration values to the <paramref name="configurationBuilder"/>.
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <param name="accelergreatEnvironmentPipelineData">See <see cref="AccelergreatEnvironmentPipelineDataKeys"/> for pre-defined keys.</param>
    protected virtual void BuildConfiguration(IConfigurationBuilder configurationBuilder, IReadOnlyAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData)
    {
    }

    /// <summary>
    /// Called after the web app has been started.
    /// </summary>
    /// <returns></returns>
    protected virtual Task OnWebAppInitializedAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called after the web app has been reset.
    /// </summary>
    protected virtual Task OnWebAppResetAsync()
    {
        return Task.CompletedTask;
    }

    async Task IAccelergreatComponent.InitializeAsync(IAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData)
    {
        var configurationBuilder = new ConfigurationBuilder();

        BuildConfiguration(configurationBuilder, accelergreatEnvironmentPipelineData);

        var configuration = configurationBuilder.Build();

        _configureWebHostAction = builder =>
        {
            builder.UseConfiguration(configuration);

            ConfigureWebHost(builder, configuration, accelergreatEnvironmentPipelineData);
        };

        await Task.Run(() => Server);

        accelergreatEnvironmentPipelineData.AddWebAppHttpClient<TEntryPoint>(CreateClient());
        accelergreatEnvironmentPipelineData.AddKestrelWebAppHttpBaseAddress<TEntryPoint>(GetBaseAddress());

        await OnWebAppInitializedAsync();
    }

    Task IAccelergreatComponent.ResetAsync()
    {
        return OnWebAppResetAsync();
    }
}