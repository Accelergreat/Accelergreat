using Microsoft.AspNetCore;

namespace Accelergreat.Tests.TestApi.Postgres;

public static class Program
{
    public static void Main(params string[] args)
    {
        CreateWebHostBuilder(args)
            .Build()
            .Run();
    }

    // ReSharper disable once MemberCanBePrivate.Global
    private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>();
}