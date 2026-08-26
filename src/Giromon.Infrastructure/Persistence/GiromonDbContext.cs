using Giromon.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Giromon.Infrastructure.Persistence;

public class GiromonDbContext : DbContext
{
    public GiromonDbContext(
        DbContextOptions<GiromonDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(GiromonDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}