using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Accelergreat.EntityFramework;

public interface IDbContextFactory<out TDbContext> : IDbContextFactoryEvents<TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    /// <para>Creates a new <typeparamref name="TDbContext"/>.</para>
    /// <para>The caller is responsible for disposing the context.</para>
    /// </summary>
    TDbContext NewDbContext();

    /// <summary>
    /// <para>Creates a new <typeparamref name="TDbContext"/>.</para>
    /// <para>The caller is responsible for disposing the context.</para>
    /// </summary>
    /// <param name="useTransactionOverriding">
    /// <para>Will only be used if the reset strategy is set to transactions in the accelergreat.json configuration file.</para>
    /// <para>If set to true, Accelergreat will intercept <see cref="DatabaseFacade.BeginTransaction()"/> <see cref="DatabaseFacade.BeginTransactionAsync(CancellationToken)"/> by returning the transaction that is used with the test database. Commits and Rollbacks will be translated to use savepoints. By default, <paramref name="useTransactionOverriding"/> is set to false.</para>
    /// </param>
    TDbContext NewDbContext(bool useTransactionOverriding);

    IDbContextFactory<TDbContext2> ChangeContextType<TDbContext2>() where TDbContext2 : DbContext;
}