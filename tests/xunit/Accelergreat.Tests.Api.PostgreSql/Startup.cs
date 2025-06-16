using Accelergreat.Tests.Api.PostgreSql.Components;
using Accelergreat.Xunit;

namespace Accelergreat.Tests.Api.PostgreSql;

internal class Startup : IAccelergreatStartup
{
    public void Configure(IAccelergreatBuilder builder)
    {
        builder.AddAccelergreatComponent<TestPostgreSqlDatabaseComponent>();
        builder.AddAccelergreatComponent<TestApiWebAppComponent>();
    }
}