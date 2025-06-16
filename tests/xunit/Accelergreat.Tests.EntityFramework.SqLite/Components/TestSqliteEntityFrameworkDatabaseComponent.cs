using System.Threading.Tasks;
using Accelergreat.EntityFramework.Sqlite;
using Accelergreat.Tests.EntityFramework.Relational.AdventureWorks;

namespace Accelergreat.Tests.EntityFramework.SqLite.Components;

public class TestSqliteSqlDatabaseComponent : SqliteEntityFrameworkDatabaseComponent<AdventureWorks2016Context>
{
    protected override Task OnDatabaseInitializedAsync(AdventureWorks2016Context context)
    {
        return Task.CompletedTask;
    }
}