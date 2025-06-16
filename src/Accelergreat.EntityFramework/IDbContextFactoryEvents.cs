using Microsoft.EntityFrameworkCore;

namespace Accelergreat.EntityFramework;

public interface IDbContextFactoryEvents<out TDbContext> where TDbContext : DbContext
{
    internal void SubscribeToOnDbContextCreatedEvent(Action<TDbContext> subscribeAction);
}