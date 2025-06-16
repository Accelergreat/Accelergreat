#if NET6_0_OR_GREATER


using System.Reflection;

namespace Accelergreat.Web.Factories.Proxies;

internal static class HostFactoryResolverProxy
{
    private static readonly Lazy<Type> ProxyType = new(() =>
    {
        var deferredHostBuilderAssembly = Assembly.Load("Microsoft.AspNetCore.Mvc.Testing");

        var deferredHostBuilderType =
            deferredHostBuilderAssembly.GetType("Microsoft.Extensions.Hosting.HostFactoryResolver")!;

        return deferredHostBuilderType;
    });

    public static Func<string[], object>? ResolveHostFactory(
        Assembly assembly,
        TimeSpan? waitTimeout = null,
        bool stopApplication = true,
        Action<object>? configureHostBuilder = null,
        Action<Exception?>? entrypointCompleted = null)
    {
        return (Func<string[], object>?) ProxyType.Value
            .GetMethod("ResolveHostFactory", BindingFlags.Public | BindingFlags.Static, new[]
            {
                typeof(Assembly),
                typeof(TimeSpan?),
                typeof(bool),
                typeof(Action<object>),
                typeof(Action<Exception>)
            })?
            .Invoke(null, new object?[]
            {
                assembly, 
                waitTimeout, 
                stopApplication, 
                configureHostBuilder, 
                entrypointCompleted
            });
    }
}

#endif