using Microsoft.EntityFrameworkCore;

namespace Accelergreat.EntityFramework;

public abstract class EntityFrameworkDatabaseComponentBase<TDbContext> where TDbContext : DbContext
{
    private IDbContextFactory<TDbContext>? _dbContextFactory;

    protected bool AutoRegisterGlobalDataItemsInDbContextCreation { get; init; } = false;

    public IReadOnlyCollection<object> GlobalDataItems { get; private set; } = Array.Empty<object>();
    public IReadOnlyCollection<T> GlobalDataItemsOfType<T>() => GlobalDataItems.OfType<T>().ToArray();

    public IDbContextFactory<TDbContext> DbContextFactory
    {
        get => _dbContextFactory ?? throw new NullReferenceException($"{nameof(_dbContextFactory)} has not been initialized.");
        private protected set
        {
            _dbContextFactory = value;
            _dbContextFactory.SubscribeToOnDbContextCreatedEvent(RegisterGlobalDataItems);
        }
    }

    internal void LoadGlobalDataItems(TDbContext context)
    {
        GlobalDataItems = context.ChangeTracker.Entries().Select(x => x.Entity).ToArray();
    }

    private void RegisterGlobalDataItems(TDbContext context)
    {
        if (AutoRegisterGlobalDataItemsInDbContextCreation)
        {
            foreach (var globalDataItem in GlobalDataItems)
            {
                var entry = context.Entry(globalDataItem);

                entry.State = EntityState.Unchanged;
            }
        }
    }
}