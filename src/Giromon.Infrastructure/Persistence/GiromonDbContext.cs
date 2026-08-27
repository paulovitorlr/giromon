using Giromon.Application.Abstractions.Persistence;
using Giromon.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Giromon.Infrastructure.Persistence;

public class GiromonDbContext : DbContext, IUnitOfWork
{
    public GiromonDbContext(
        DbContextOptions<GiromonDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<WalletTransaction> WalletTransactions =>
        Set<WalletTransaction>();
    public DbSet<GameRound> GameRounds => Set<GameRound>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(GiromonDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}