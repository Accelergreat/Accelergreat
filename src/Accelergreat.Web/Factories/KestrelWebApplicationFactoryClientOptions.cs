using Microsoft.AspNetCore.Mvc.Testing.Handlers;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Accelergreat.Web.Factories;

public class KestrelWebApplicationFactoryClientOptions
{
    /// <summary>
    /// Gets or sets whether or not <see cref="HttpClient"/> instances created by calling
    /// <see cref="KestrelWebApplicationFactory{TEntryPoint}.CreateClient(KestrelWebApplicationFactoryClientOptions)"/>
    /// should automatically follow redirect responses.
    /// The default is <c>true</c>.
    /// /// </summary>
    public bool AllowAutoRedirect { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum number of redirect responses that <see cref="HttpClient"/> instances
    /// created by calling <see cref="KestrelWebApplicationFactory{TEntryPoint}.CreateClient(KestrelWebApplicationFactoryClientOptions)"/>
    /// should follow.
    /// The default is <c>7</c>.
    /// </summary>
    public int MaxAutomaticRedirections { get; set; } = 7;

    /// <summary>
    /// Gets or sets whether <see cref="HttpClient"/> instances created by calling
    /// <see cref="KestrelWebApplicationFactory{TEntryPoint}.CreateClient(KestrelWebApplicationFactoryClientOptions)"/>
    /// should handle cookies.
    /// The default is <c>true</c>.
    /// </summary>
    public bool HandleCookies { get; set; } = true;

    internal DelegatingHandler[] CreateHandlers()
    {
        return CreateHandlersCore().ToArray();

        IEnumerable<DelegatingHandler> CreateHandlersCore()
        {
            if (AllowAutoRedirect)
            {
                yield return new RedirectHandler(MaxAutomaticRedirections);
            }
            if (HandleCookies)
            {
                yield return new CookieContainerHandler();
            }
        }
    }
}