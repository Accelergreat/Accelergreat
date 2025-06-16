using System.Reflection;
using System.Text;
using System.Text.Json;
using Accelergreat.Networking;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
// ReSharper disable MemberCanBePrivate.Global

#if NET6_0_OR_GREATER
using Microsoft.Extensions.Configuration;
using Accelergreat.Web.Factories.Proxies;
// ReSharper disable MemberCanBeProtected.Global
#endif

namespace Accelergreat.Web.Factories
{
    public class KestrelWebApplicationFactory<TEntryPoint> : IDisposable, IAsyncDisposable where TEntryPoint : class
    {
        private bool _disposed;
        private bool _disposedAsync;
#if NET6_0_OR_GREATER
        private IHost? _host;
#endif
        private IWebHost? _webHost;
        private IServer? _server;
        // ReSharper disable once ArrangeObjectCreationWhenTypeEvident
        private readonly List<HttpClient> _clients = new List<HttpClient>();

        private protected KestrelWebApplicationFactory()
        {
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="KestrelWebApplicationFactory{TEntryPoint}"/> class.
        /// </summary>
        ~KestrelWebApplicationFactory()
        {
            Dispose(false);
        }

        protected bool ConfigureHttpsPort { get; set; } = false;

        /// <summary>
        /// Gets the <see cref="IServer"/> created by this <see cref="WebApplicationFactory{TEntryPoint}"/>.
        /// </summary>
        public IServer Server
        {
            get
            {
                EnsureServer();
                return _server!;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="preferHttps"></param>
        /// <returns></returns>
        public string GetBaseAddress(bool preferHttps = true)
        {
            EnsureServer();

            // ReSharper disable once RedundantSuppressNullableWarningExpression
            var serverAddressesFeature = _server!.Features.Get<IServerAddressesFeature>()!;

            var addresses = serverAddressesFeature.Addresses;

            var preferredBaseAddress = preferHttps
                ? addresses.FirstOrDefault(x =>
                    x.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                : addresses.FirstOrDefault(x =>
                    x.StartsWith("http://", StringComparison.OrdinalIgnoreCase));


            return preferredBaseAddress ?? addresses.First();
        }

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> created by the server associated with this <see cref="KestrelWebApplicationFactory{TEntryPoint}"/>.
        /// </summary>
        public virtual IServiceProvider? Services
        {
            get
            {
                EnsureServer();

#if NET6_0_OR_GREATER
                return _webHost?.Services ?? _host?.Services;
#else
                return _webHost?.Services;
#endif
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="HttpClient"/> that automatically follows
        /// redirects and handles cookies.
        /// </summary>
        /// <returns>The <see cref="HttpClient"/>.</returns>
        public HttpClient CreateClient() =>
            CreateClient(new KestrelWebApplicationFactoryClientOptions());

        /// <summary>
        /// Creates an instance of <see cref="HttpClient"/> that automatically follows
        /// redirects and handles cookies.
        /// </summary>
        /// <returns>The <see cref="HttpClient"/>.</returns>
        public HttpClient CreateClient(KestrelWebApplicationFactoryClientOptions options) =>
            CreateDefaultClient(options.CreateHandlers());

        /// <summary>
        /// Creates a new instance of an <see cref="HttpClient"/> that can be used to
        /// send <see cref="HttpRequestMessage"/> to the server.
        /// </summary>
        /// <param name="handlers">A list of <see cref="DelegatingHandler"/> instances to set up on the
        /// <see cref="HttpClient"/>.</param>
        /// <returns>The <see cref="HttpClient"/>.</returns>
        public HttpClient CreateDefaultClient(params DelegatingHandler[] handlers)
        {
            EnsureServer();

            HttpClient client;
            if (!handlers.Any())
            {
                client = new HttpClient();
            }
            else
            {
                for (var i = handlers.Length - 1; i > 0; i--)
                {
                    handlers[i - 1].InnerHandler = handlers[i];
                }

                handlers[^1].InnerHandler = new SocketsHttpHandler();

                client = new HttpClient(handlers[0]);
            }

            _clients.Add(client);

            ConfigureClient(client);

            return client;
        }

        /// <summary>
        /// Creates a <see cref="IWebHostBuilder"/> used to set up <see cref="KestrelServer"/>.
        /// </summary>
        /// <remarks>
        /// The default implementation of this method looks for a <c>public static IWebHostBuilder CreateWebHostBuilder(string[] args)</c>
        /// method defined on the entry point of the assembly of <typeparamref name="TEntryPoint" /> and invokes it passing an empty string
        /// array as arguments.
        /// </remarks>
        /// <returns>A <see cref="IWebHostBuilder"/> instance.</returns>
        protected virtual IWebHostBuilder? CreateWebHostBuilder()
        {
            var builder = WebHostBuilderFactory.CreateFromTypesAssemblyEntryPoint<TEntryPoint>(Array.Empty<string>());

            return builder?.UseEnvironment(Microsoft.Extensions.Hosting.Environments.Development);
        }

        /// <summary>
        /// Creates the <see cref="IHost"/> with the bootstrapped application in <paramref name="builder"/>.
        /// This is only called for applications using <see cref="IHostBuilder"/>. Applications based on
        /// <see cref="IWebHostBuilder"/> will use <see cref="CreateWebHost"/> instead.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> used to create the host.</param>
        /// <returns>The <see cref="IHost"/> with the bootstrapped application.</returns>
        protected virtual IHost CreateHost(IHostBuilder builder)
        {
            var host = builder.Build();
            return host;
        }

        /// <summary>
        /// Creates the <see cref="IWebHost"/> with the bootstrapped application in <paramref name="builder"/>.
        /// This is only called for applications using <see cref="IHostBuilder"/>. Applications based on
        /// <see cref="IHostBuilder"/> will use <see cref="CreateHost"/> instead.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> used to create the host.</param>
        /// <returns>The <see cref="IHost"/> with the bootstrapped application.</returns>
        protected virtual IWebHost CreateWebHost(IWebHostBuilder builder)
        {
            var host = builder.Build();
            return host;
        }

        /// <summary>
        /// Gives a fixture an opportunity to configure the application before it gets built.
        /// </summary>
        /// <param name="builder">The <see cref="IWebHostBuilder"/> for the application.</param>
        protected virtual void ConfigureWebHost(IWebHostBuilder builder)
        {
        }

        /// <summary>
        /// Configures <see cref="HttpClient"/> instances created by this <see cref="KestrelWebApplicationFactory{TEntryPoint}"/>.
        /// </summary>
        /// <param name="client">The <see cref="HttpClient"/> instance getting configured.</param>
        protected virtual void ConfigureClient(HttpClient client)
        {
            client.BaseAddress = new Uri(GetBaseAddress());
        }

        private void EnsureServer()
        {
            if (_server is not null)
            {
                return;
            }

            EnsureDepsFile();

            var webHostBuilder = CreateWebHostBuilder();

#if NET6_0_OR_GREATER

            if (webHostBuilder is null)
            {
                var deferredHostBuilder = new DeferredHostBuilderProxy();

                deferredHostBuilder.UseEnvironment(Microsoft.Extensions.Hosting.Environments.Development);

                // There's no helper for UseApplicationName, but we need to 
                // set the application name to the target entry point 
                // assembly name.
                deferredHostBuilder.ConfigureHostConfiguration(config =>
                {
                    config.AddInMemoryCollection(new[] { new KeyValuePair<string, string?>(HostDefaults.ApplicationKey, typeof(TEntryPoint).Assembly.GetName().Name ?? string.Empty) });
                });

                // This helper call does the hard work to determine if we can fallback to diagnostic source events to get the host instance
                var factory = HostFactoryResolverProxy.ResolveHostFactory(
                    typeof(TEntryPoint).Assembly,
                    stopApplication: false,
                    configureHostBuilder: deferredHostBuilder.ConfigureHostBuilder,
                    entrypointCompleted: deferredHostBuilder.EntryPointCompleted);
                
                if (factory is not null)
                {
                    // If we have a valid factory it means the specified entry point's assembly can potentially resolve the IHost
                    // so we set the factory on the DeferredHostBuilder so we can invoke it on the call to IHostBuilder.Build.
                    deferredHostBuilder.SetHostFactory(factory);

                    ConfigureHostBuilder(deferredHostBuilder);
                    return;
                }

                throw new InvalidOperationException("Missing builder method");
            }

#endif

            // ReSharper disable once RedundantSuppressNullableWarningExpression
            ConfigureWebHostBuilder(webHostBuilder!);

            // ReSharper disable once RedundantSuppressNullableWarningExpression
            _webHost = CreateWebHost(webHostBuilder!);

            _webHost!.Start();

            _server = _webHost.Services.GetRequiredService<IServer>();
        }

#if NET6_0_OR_GREATER

        private void ConfigureHostBuilder(IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureWebHost(ConfigureWebHostBuilder);

            _host = CreateHost(hostBuilder);

            _host!.Start();

            _server = _host.Services.GetRequiredService<IServer>();
        }

#endif

        private void ConfigureWebHostBuilder(IWebHostBuilder webHostBuilder)
        {
            SetContentRoot(webHostBuilder);

            ConfigureWebHost(webHostBuilder);

            var urlsBuilder = new StringBuilder($"http://localhost:{AccelergreatPortProvider.GetNextAvailablePort()}");

            if (ConfigureHttpsPort)
            {
                urlsBuilder.Append($";https://localhost:{AccelergreatPortProvider.GetNextAvailablePort()}");
            }

            webHostBuilder
                .UseKestrel()
                .UseUrls(urlsBuilder.ToString());
        }

        private void SetContentRoot(IWebHostBuilder builder)
        {
            if (SetContentRootFromSetting(builder))
            {
                return;
            }

            var fromFile = File.Exists("MvcTestingAppManifest.json");
            var contentRoot = fromFile ? GetContentRootFromFile("MvcTestingAppManifest.json") : GetContentRootFromAssembly();

            if (contentRoot is not null)
            {
                builder.UseContentRoot(contentRoot);
            }
            else
            {
                builder.UseSolutionRelativeContentRoot(typeof(TEntryPoint).Assembly.GetName().Name!);
            }
        }

        private static string? GetContentRootFromFile(string file)
        {
            var data = JsonSerializer.Deserialize<IDictionary<string, string>>(File.ReadAllBytes(file))!;
            var key = typeof(TEntryPoint).Assembly.GetName().FullName;

            // If the `ContentRoot` is not provided in the app manifest, then return null
            // and fallback to setting the content root relative to the entrypoint's assembly.
            if (!data.TryGetValue(key, out var contentRoot))
            {
                return null;
            }

            return (contentRoot == "~") ? AppContext.BaseDirectory : contentRoot;
        }

        private string? GetContentRootFromAssembly()
        {
            var metadataAttributes = GetContentRootMetadataAttributes(
                typeof(TEntryPoint).Assembly.FullName!,
                typeof(TEntryPoint).Assembly.GetName().Name!);

            string? contentRoot = null;
            for (var i = 0; i < metadataAttributes.Length; i++)
            {
                var contentRootAttribute = metadataAttributes[i];
                var contentRootCandidate = Path.Combine(
                    AppContext.BaseDirectory,
                    contentRootAttribute.ContentRootPath);

                var contentRootMarker = Path.Combine(
                    contentRootCandidate,
                    Path.GetFileName(contentRootAttribute.ContentRootTest));

                if (File.Exists(contentRootMarker))
                {
                    contentRoot = contentRootCandidate;
                    break;
                }
            }

            return contentRoot;
        }

        private static bool SetContentRootFromSetting(IWebHostBuilder builder)
        {
            // Attempt to look for TEST_CONTENTROOT_APPNAME in settings. This should result in looking for
            // ASPNETCORE_TEST_CONTENTROOT_APPNAME environment variable.
            var assemblyName = typeof(TEntryPoint).Assembly.GetName().Name!;
            var settingSuffix = assemblyName.ToUpperInvariant().Replace(".", "_");
            var settingName = $"TEST_CONTENTROOT_{settingSuffix}";

            var settingValue = builder.GetSetting(settingName);
            if (settingValue is null)
            {
                return false;
            }

            builder.UseContentRoot(settingValue);
            return true;
        }

        private WebApplicationFactoryContentRootAttribute[] GetContentRootMetadataAttributes(
            string tEntryPointAssemblyFullName,
            string tEntryPointAssemblyName)
        {
            var testAssembly = GetTestAssemblies();
            var metadataAttributes = testAssembly
                .SelectMany(a => a.GetCustomAttributes<WebApplicationFactoryContentRootAttribute>())
                .Where(a => string.Equals(a.Key, tEntryPointAssemblyFullName, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(a.Key, tEntryPointAssemblyName, StringComparison.OrdinalIgnoreCase))
                .OrderBy(a => a.Priority)
                .ToArray();

            return metadataAttributes;
        }

        /// <summary>
        /// Gets the assemblies containing the functional tests. The
        /// <see cref="WebApplicationFactoryContentRootAttribute"/> applied to these
        /// assemblies defines the content root to use for the given
        /// <typeparamref name="TEntryPoint"/>.
        /// </summary>
        /// <returns>The list of <see cref="Assembly"/> containing tests.</returns>
        protected virtual IEnumerable<Assembly> GetTestAssemblies()
        {
            try
            {
                // The default dependency context will be populated in .net core applications.
                var context = DependencyContext.Default;
                if (context is null || context.CompileLibraries.Count == 0)
                {
                    // The app domain friendly name will be populated in full framework.
                    return new[] { Assembly.Load(AppDomain.CurrentDomain.FriendlyName) };
                }

                var runtimeProjectLibraries = context.RuntimeLibraries
                    .ToDictionary(r => r.Name, r => r, StringComparer.Ordinal);

                var entryPointAssemblyName = typeof(TEntryPoint).Assembly.GetName().Name;

                // Find the list of projects referencing TEntryPoint.
                var candidates = context.CompileLibraries
                    .Where(library => library.Dependencies.Any(d => string.Equals(d.Name, entryPointAssemblyName, StringComparison.Ordinal)));

                var testAssemblies = new List<Assembly>();
                foreach (var candidate in candidates)
                {
                    if (runtimeProjectLibraries.TryGetValue(candidate.Name, out var runtimeLibrary))
                    {
                        var runtimeAssemblies = runtimeLibrary.GetDefaultAssemblyNames(context);
                        testAssemblies.AddRange(runtimeAssemblies.Select(Assembly.Load));
                    }
                }

                return testAssemblies;
            }
            catch (Exception)
            {
                // ignored due to Microsoft code
            }

            return Array.Empty<Assembly>();
        }

        private static void EnsureDepsFile()
        {
            if (typeof(TEntryPoint).Assembly.EntryPoint is null)
            {
                throw new InvalidOperationException("Invalid Entry point");
            }

            var depsFileName = $"{typeof(TEntryPoint).Assembly.GetName().Name}.deps.json";
            var depsFile = new FileInfo(Path.Combine(AppContext.BaseDirectory, depsFileName));
            if (!depsFile.Exists)
            {
                throw new InvalidOperationException("Missing deps file");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true" /> to release both managed and unmanaged resources;
        /// <see langword="false" /> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing && !_disposedAsync)
            {
                DisposeAsync()
                    .AsTask()
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            }

            _disposed = true;
        }

        public virtual async ValueTask DisposeAsync()
        {
            if (_disposed)
            {
                return;
            }

            if (_disposedAsync)
            {
                return;
            }

            foreach (var client in _clients)
            {
                client.Dispose();
            }

            if (_webHost is not null)
            {
                await _webHost.StopAsync().ConfigureAwait(false);
                _webHost?.Dispose();
            }

#if NET6_0_OR_GREATER

            if (_host is not null)
            {
                await _host.StopAsync().ConfigureAwait(false);
                _host?.Dispose();
            }

#endif

            _disposedAsync = true;

            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }
    }
}
