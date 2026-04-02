# Accelergreat.EntityFramework.Sqlite

SQLite database component support for Accelergreat integration tests.

## Install

```bash
dotnet add package Accelergreat.EntityFramework.Sqlite
```

## Typical component

```csharp
public class AppDatabaseComponent : SqliteEntityFrameworkDatabaseComponent<AppDbContext>
{
    public AppDatabaseComponent(IConfiguration configuration) : base(configuration)
    {
    }
}
```

## Common usage notes

- Good fit for fast integration tests with lightweight DB requirements.
- Useful when full SQL Server behavior is not required for the test scenario.

## Troubleshooting

- **Schema not present**
  - verify create strategy and initialization logic in component setup
- **Data persistence surprises**
  - verify how the SQLite DB is configured (in-memory vs file-backed) in your test configuration

## FAQ

- **When should I choose SQLite over SQL Server package?**
  - Choose SQLite for lightweight tests where provider-specific SQL Server behavior is not required.
- **Can I still use the same Accelergreat test setup?**
  - Yes. Swap the DB component base type and related package reference.

## Related docs

- [`../../README.md`](../../README.md)
- [`../Accelergreat.EntityFramework/README.md`](../Accelergreat.EntityFramework/README.md)
