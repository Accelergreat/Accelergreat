using Accelergreat.Environments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Accelergreat.EntityFramework.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAccelergreatDbContext<TDbContext>(
        this IServiceCollection services,
        IReadOnlyAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData,
        bool useTransactionOverriding = false,
        ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        where TDbContext : DbContext
    {
        return services.AddAccelergreatDbContext<TDbContext, TDbContext>(
            accelergreatEnvironmentPipelineData, useTransactionOverriding, serviceLifetime);
    }

    public static IServiceCollection AddAccelergreatDbContext<TDbContext, TRegisterAsDbContext>(
        this IServiceCollection services,
        IReadOnlyAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData,
        bool useTransactionOverriding = false,
        ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        where TDbContext : DbContext, TRegisterAsDbContext
        where TRegisterAsDbContext : DbContext
    {
        if (typeof(TRegisterAsDbContext) == typeof(DbContext))
        {
            services.RemoveAll<DbContextOptions>();
        }
        else
        {
            services.RemoveAll<DbContextOptions<TRegisterAsDbContext>>();
        }

        services.RemoveAll<TRegisterAsDbContext>();

        var dbContextFactory = accelergreatEnvironmentPipelineData.GetEntityFrameworkDbContextFactory<TDbContext>();

        services.Add(new ServiceDescriptor(
            typeof(TRegisterAsDbContext),
            _ => dbContextFactory.NewDbContext(useTransactionOverriding),
            serviceLifetime));

        return services;
    }

    public static IServiceCollection AddAccelergreatProxiedDbContext<TDbContext, TRegisterAsDbContext>(
        this IServiceCollection services,
        IReadOnlyAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData,
        bool useTransactionOverriding = false,
        ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        where TDbContext : DbContext
        where TRegisterAsDbContext : DbContext
    {
        if (typeof(TRegisterAsDbContext) == typeof(DbContext))
        {
            services.RemoveAll<DbContextOptions>();
        }
        else
        {
            services.RemoveAll<DbContextOptions<TRegisterAsDbContext>>();
        }

        services.RemoveAll<TRegisterAsDbContext>();

        var dbContextFactory = accelergreatEnvironmentPipelineData.GetEntityFrameworkDbContextFactory<TDbContext>();

        var newDbContextFactory = dbContextFactory.ChangeContextType<TRegisterAsDbContext>();

        services.Add(new ServiceDescriptor(
            typeof(TRegisterAsDbContext),
            _ => newDbContextFactory.NewDbContext(useTransactionOverriding),
            serviceLifetime));

        return services;
    }

    public static IServiceCollection AddAccelergreatDbContextWithTransactionOverriding<TDbContext>(
        this IServiceCollection services,
        IReadOnlyAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData,
        ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        where TDbContext : DbContext
    {
        return services
            .AddAccelergreatDbContextWithTransactionOverriding<TDbContext, TDbContext>(
                accelergreatEnvironmentPipelineData,
                serviceLifetime);
    }

    public static IServiceCollection AddAccelergreatDbContextWithTransactionOverriding<TDbContext, TRegisterAsDbContext>(
        this IServiceCollection services,
        IReadOnlyAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData,
        ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        where TDbContext : DbContext, TRegisterAsDbContext
        where TRegisterAsDbContext : DbContext
    {
        return services.AddAccelergreatDbContext<TDbContext, TRegisterAsDbContext>(
            accelergreatEnvironmentPipelineData,
            true,
            serviceLifetime);
    }

    public static IServiceCollection AddAccelergreatProxiedDbContextWithTransactionOverriding<TDbContext, TRegisterAsDbContext>(
        this IServiceCollection services,
        IReadOnlyAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData,
        ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        where TDbContext : DbContext
        where TRegisterAsDbContext : DbContext
    {
        return services.AddAccelergreatProxiedDbContext<TDbContext, TRegisterAsDbContext>(
            accelergreatEnvironmentPipelineData,
            true,
            serviceLifetime);
    }
}