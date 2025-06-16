using Microsoft.EntityFrameworkCore;

namespace Accelergreat.EntityFramework;

internal class OnDbContextCreatedEventArgs<TDbContext> : EventArgs where TDbContext : DbContext
{
    internal OnDbContextCreatedEventArgs(TDbContext context)
    {
        Context = context;
    }

    internal TDbContext Context { get; }
}