# Accelergreat.EntityFramework

Shared Entity Framework integration primitives used by provider-specific Accelergreat packages.

## What this package provides

- base abstractions for database-backed components
- reset/create strategy model shared by SQL Server and SQLite providers
- shared extension points for DB component behavior

## Typical install pattern

Most users should install a provider package directly, not this package alone:

- `Accelergreat.EntityFramework.SqlServer`
- `Accelergreat.EntityFramework.Sqlite`

Plus a test framework integration package:

- `Accelergreat.Xunit` or `Accelergreat.Xunit3`

## When to reference this package directly

Reference `Accelergreat.EntityFramework` directly only when building custom provider integrations or shared abstractions on top of Accelergreat.

## FAQ

- **Should I install this package without a provider package?**
  - Usually no. Most users should install `Accelergreat.EntityFramework.SqlServer` or `Accelergreat.EntityFramework.Sqlite`.
- **Can I switch providers later?**
  - Yes. The test-framework integration remains the same; provider-specific package and component types change.

## Related docs

- [`../../README.md`](../../README.md)
- [`../Accelergreat.EntityFramework.SqlServer/README.md`](../Accelergreat.EntityFramework.SqlServer/README.md)
- [`../Accelergreat.EntityFramework.Sqlite/README.md`](../Accelergreat.EntityFramework.Sqlite/README.md)
