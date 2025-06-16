using Accelergreat.Components;
using Microsoft.EntityFrameworkCore;

namespace Accelergreat.EntityFramework;

/// <summary>
/// A component for a database managed via Entity Framework.
/// </summary>
public interface IEntityFrameworkDatabaseComponent<out TDbContext> : IAccelergreatComponent
    where TDbContext : DbContext
{
    IDbContextFactory<TDbContext> DbContextFactory { get; }
}