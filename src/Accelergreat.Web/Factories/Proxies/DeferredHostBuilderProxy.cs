#if NET6_0_OR_GREATER


using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Accelergreat.Web.Factories.Proxies;

internal class DeferredHostBuilderProxy : IHostBuilder
{
    private static readonly Lazy<Type> ProxyType = new(() =>
    {
        var deferredHostBuilderAssembly = Assembly.Load("Microsoft.AspNetCore.Mvc.Testing");

        var deferredHostBuilderType =
            deferredHostBuilderAssembly.GetType("Microsoft.AspNetCore.Mvc.Testing.DeferredHostBuilder")!;

        return deferredHostBuilderType;
    });

    private readonly IHostBuilder _hostBuilder;

    public DeferredHostBuilderProxy()
    {
        _hostBuilder = (IHostBuilder) ProxyType.Value.GetConstructor(Type.EmptyTypes)!.Invoke(Array.Empty<object>());
    }

    public void ConfigureHostBuilder(object hostBuilder)
    {
        ProxyType.Value
            .GetMethod("ConfigureHostBuilder", BindingFlags.Public | BindingFlags.Instance,
                new[] { typeof(object) })?
            .Invoke(_hostBuilder, new[] { hostBuilder });
    }

    public void EntryPointCompleted(Exception? exception)
    {
        ProxyType.Value
            .GetMethod("EntryPointCompleted", BindingFlags.Public | BindingFlags.Instance,
                new[] { typeof(Exception) })?
            .Invoke(_hostBuilder, new object?[] { exception });
    }

    public void SetHostFactory(Func<string[], object> factory)
    {
        ProxyType.Value
            .GetMethod("SetHostFactory", BindingFlags.Public | BindingFlags.Instance,
                new[] { typeof(Func<string[], object>) })?
            .Invoke(_hostBuilder, new object?[] { factory });
    }

    public IHost Build() => 
        _hostBuilder.Build();

    public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate) =>
        _hostBuilder.ConfigureAppConfiguration(configureDelegate);

    public IHostBuilder ConfigureContainer<TContainerBuilder>(
        Action<HostBuilderContext, TContainerBuilder> configureDelegate) =>
        _hostBuilder.ConfigureContainer(configureDelegate);

    public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate) =>
        _hostBuilder.ConfigureHostConfiguration(configureDelegate);

    public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate) =>
        _hostBuilder.ConfigureServices(configureDelegate);

    public IHostBuilder
        UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) where TContainerBuilder: notnull =>
        _hostBuilder.UseServiceProviderFactory(factory);

    public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(
        Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory) where TContainerBuilder : notnull =>
        _hostBuilder.UseServiceProviderFactory(factory);

    public IDictionary<object, object> Properties => 
        _hostBuilder.Properties;
}

#endif