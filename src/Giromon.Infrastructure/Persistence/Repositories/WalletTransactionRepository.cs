using Giromon.Application.Abstractions.Persistence;
using Giromon.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Giromon.Infrastructure.Persistence.Repositories;

public class WalletTransactionRepository
    : IWalletTransactionRepository
{
    private readonly GiromonDbContext _dbContext;

    public WalletTransactionRepository(
        GiromonDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        WalletTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.WalletTransactions.AddAsync(
            transaction,
            cancellationToken);
    }

    public async Task<IReadOnlyList<WalletTransaction>>
        GetByWalletIdAsync(
            Guid walletId,
            CancellationToken cancellationToken = default)
    {
        return await _dbContext.WalletTransactions
            .AsNoTracking()
            .Where(transaction =>
                transaction.WalletId == walletId)
            .OrderByDescending(transaction =>
                transaction.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}