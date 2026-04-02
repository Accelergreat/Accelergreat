# Accelergreat.EntityFramework.SqlServer

SQL Server database component support for Accelergreat integration tests.

## Install

```bash
dotnet add package Accelergreat.EntityFramework.SqlServer
```

## Prerequisites

- SQL Server reachable from test environment
- connection string configured for test usage

## Typical component

```csharp
public class AppDatabaseComponent : SqlServerEntityFrameworkDatabaseComponent<AppDbContext>
{
    public AppDatabaseComponent(IConfiguration configuration) : base(configuration)
    {
    }
}
```

## Common configuration

```json
{
  "SqlServerEntityFramework": {
    "ResetStrategy": "Transactions",
    "CreateStrategy": "Migrations"
  }
}
```

## Reset strategy guidance

- `Transactions`: fastest reset, best local developer feedback loop.
- `SnapshotRollback`: slower but robust reset model, often preferred in CI.

## Troubleshooting

- **Database does not reset as expected**
  - verify `ResetStrategy` value in active Accelergreat config file
  - verify test project loads intended environment configuration
- **Connection issues**
  - validate SQL Server is reachable from test host
  - verify credentials and DB permissions

## FAQ

- **Which reset strategy should I start with?**
  - Start with `Transactions` for local speed; switch to `SnapshotRollback` in CI if needed.
- **Do I need migrations?**
  - Use `CreateStrategy: Migrations` when your test DB should mirror migration history.

## Related docs

- [`../../README.md`](../../README.md)
- [`../Accelergreat.EntityFramework/README.md`](../Accelergreat.EntityFramework/README.md)
