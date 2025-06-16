using System.ComponentModel;
using Accelergreat.Components;
using Accelergreat.Xunit.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Accelergreat.Xunit;


internal sealed class AccelergreatBuilder
    : IAccelergreatBuilder
{
    internal AccelergreatBuilder(IServiceCollection services, IConfiguration configuration)
    {
        Services = services;
        Configuration = configuration;
    }

    public IServiceCollection Services { get; }
    
    public IConfiguration Configuration { get; }
    
    public IAccelergreatBuilder AddSingletonAccelergreatComponent<TAccelergreatComponent>() where TAccelergreatComponent : class, IAccelergreatComponent
    {
        Services.AddSingletonAccelergreatComponent<TAccelergreatComponent>();
        return this;
    }

    public IAccelergreatBuilder AddTransientAccelergreatComponent<TAccelergreatComponent>() where TAccelergreatComponent : class, IAccelergreatComponent
    {
        Services.AddTransientAccelergreatComponent<TAccelergreatComponent>();
        return this;
    }

    public IAccelergreatBuilder AddAccelergreatComponent<TAccelergreatComponent>() where TAccelergreatComponent : class, IAccelergreatComponent
    {
        Services.AddAccelergreatComponent<TAccelergreatComponent>();
        return this;
    }
}