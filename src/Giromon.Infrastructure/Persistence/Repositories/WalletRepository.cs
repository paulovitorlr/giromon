using Giromon.Application.Abstractions.Persistence;
using Giromon.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Giromon.Infrastructure.Persistence.Repositories;

public class WalletRepository : IWalletRepository
{
    private readonly GiromonDbContext _dbContext;

    public WalletRepository(GiromonDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Wallet?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Wallets
            .SingleOrDefaultAsync(
                wallet => wallet.UserId == userId,
                cancellationToken);
    }

    public async Task AddAsync(
        Wallet wallet,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Wallets.AddAsync(
            wallet,
            cancellationToken);
    }
}