using System.ComponentModel;
using Accelergreat.EntityFramework.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Accelergreat.EntityFramework;


internal abstract class DbContextFactoryBase<TDbContext> : IDbContextFactoryEvents<TDbContext>
    where TDbContext : DbContext
{
    private event EventHandler<OnDbContextCreatedEventArgs<TDbContext>>? OnDbContextCreated;

    public TDbContext NewDbContext()
    {
        if (!(typeof(TDbContext).GetConstructor(new[] { typeof(DbContextOptions<TDbContext>) })
                is { } dbContextConstructor))
        {
            throw new AccelergreatDbContextInstantiationException<TDbContext>();
        }

        var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();

        ConfigureOptions(optionsBuilder);

        var context = (TDbContext)dbContextConstructor.Invoke(new object[] { optionsBuilder.Options });

        ConfigureContext(context);

        TriggerOnDbContextCreatedEvent(context);

        return context;
    }


    public virtual TDbContext NewDbContext(bool useTransactionOverriding)
    {
        return NewDbContext();
    }

    internal virtual void ConfigureOptions(DbContextOptionsBuilder<TDbContext> optionsBuilder)
    {
    }

    protected virtual void ConfigureContext(TDbContext context)
    {
    }

    private protected void TriggerOnDbContextCreatedEvent(TDbContext context)
    {
        OnDbContextCreated?.Invoke(this, new OnDbContextCreatedEventArgs<TDbContext>(context));
    }

    void IDbContextFactoryEvents<TDbContext>.SubscribeToOnDbContextCreatedEvent(Action<TDbContext> subscribeAction)
    {
        OnDbContextCreated += (_, args) => subscribeAction(args.Context);
    }
}