using Accelergreat.Components;
using Microsoft.Extensions.DependencyInjection;
// ReSharper disable MemberCanBePrivate.Global

namespace Accelergreat.Xunit.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSingletonAccelergreatComponent<TAccelergreatComponent>(this IServiceCollection services)
        where TAccelergreatComponent : class, IAccelergreatComponent
    {
        if (services.Any(x => x.ImplementationType == typeof(TAccelergreatComponent)))
        {
            throw new InvalidOperationException($"{typeof(TAccelergreatComponent).Name} has already been added");
        }

        services.AddSingleton<TAccelergreatComponent>();
        services.AddSingleton<IAccelergreatComponent>(serviceProvider =>
            new AccelergreatComponentDecorator(
                serviceProvider.GetRequiredService<TAccelergreatComponent>(),
                ServiceLifetime.Singleton));

        return services;
    }

    public static IServiceCollection AddTransientAccelergreatComponent<TAccelergreatComponent>(this IServiceCollection services)
        where TAccelergreatComponent : class, IAccelergreatComponent
    {
        if (services.Any(x => x.ImplementationType == typeof(TAccelergreatComponent)))
        {
            throw new InvalidOperationException($"{typeof(TAccelergreatComponent).Name} has already been added");
        }

        services.AddTransient<TAccelergreatComponent>();
        services.AddTransient<IAccelergreatComponent>(serviceProvider =>
            new AccelergreatComponentDecorator(
                serviceProvider.GetRequiredService<TAccelergreatComponent>(),
                ServiceLifetime.Transient));

        return services;
    }

    public static IServiceCollection AddAccelergreatComponent<TAccelergreatComponent>(this IServiceCollection services)
        where TAccelergreatComponent : class, IAccelergreatComponent
    {
        return services.AddTransientAccelergreatComponent<TAccelergreatComponent>();
    }
}