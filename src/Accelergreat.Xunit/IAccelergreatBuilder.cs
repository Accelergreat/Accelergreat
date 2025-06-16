using Accelergreat.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Accelergreat.Xunit;

public interface IAccelergreatBuilder
{
    IServiceCollection Services { get; }
    
    IConfiguration Configuration { get; }

    IAccelergreatBuilder AddSingletonAccelergreatComponent<TAccelergreatComponent>()
        where TAccelergreatComponent : class, IAccelergreatComponent;
    
    IAccelergreatBuilder AddTransientAccelergreatComponent<TAccelergreatComponent>()
        where TAccelergreatComponent : class, IAccelergreatComponent;
    
    IAccelergreatBuilder AddAccelergreatComponent<TAccelergreatComponent>()
        where TAccelergreatComponent : class, IAccelergreatComponent;
}