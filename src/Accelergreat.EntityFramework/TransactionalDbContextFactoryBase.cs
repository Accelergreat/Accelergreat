using System.ComponentModel;
using Accelergreat.EntityFramework.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Accelergreat.EntityFramework;


internal abstract class TransactionalDbContextFactoryBase<TDbContext> : DbContextFactoryBase<TDbContext>
    where TDbContext : DbContext
{
    public override TDbContext NewDbContext(bool useTransactionOverriding)
    {
        if (!(typeof(TDbContext).GetConstructor(new[] { typeof(DbContextOptions<TDbContext>) })
                is { } dbContextConstructor))
        {
            throw new AccelergreatDbContextInstantiationException<TDbContext>();
        }

        var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();

        ConfigureOptions(optionsBuilder);

        if (useTransactionOverriding)
        {
            SetupTransactionOverriding(optionsBuilder);
        }

        var context = (TDbContext)dbContextConstructor.Invoke(new object[] { optionsBuilder.Options });

        ConfigureContext(context);

        TriggerOnDbContextCreatedEvent(context);

        return context;
    }

    internal abstract void SetupTransactionOverriding(DbContextOptionsBuilder<TDbContext> optionsBuilder);
}