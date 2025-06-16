using Microsoft.EntityFrameworkCore;

namespace Accelergreat.Tests.EntityFramework.SqlServer.MigrationsDatabase;

public class TestMigrationsDbContext : DbContext
{
    public TestMigrationsDbContext()
    {
            
    }

    public TestMigrationsDbContext(DbContextOptions<TestMigrationsDbContext> dbContextOptions)
        : base(dbContextOptions)
    {
            
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer();
        base.OnConfiguring(optionsBuilder);
    }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public DbSet<Blog> Blogs { get; set; } = default!;
}

public class Blog
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
}