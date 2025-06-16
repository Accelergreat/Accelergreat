using Accelergreat.Tests.EntityFramework.Relational.AdventureWorks;
using Microsoft.EntityFrameworkCore;

namespace Accelergreat.Tests.TestApi.Postgres;

public class Startup
{
    // ReSharper disable once NotAccessedField.Local
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;

    public Startup(IWebHostEnvironment env, IConfiguration configuration)
    {
        _env = env;
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        if (_env.EnvironmentName != "IntegrationTesting")
        {
            services.AddDbContext<AdventureWorks2016Context>(options =>
            {
                options.UseNpgsql(_configuration.GetConnectionString("TestDatabase"));
            });
        }
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}