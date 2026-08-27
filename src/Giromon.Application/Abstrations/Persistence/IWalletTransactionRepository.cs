using Giromon.Domain.Entities;

namespace Giromon.Application.Abstractions.Persistence;

public interface IWalletTransactionRepository
{
    Task AddAsync(
        WalletTransaction transaction,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WalletTransaction>> GetByWalletIdAsync(
        Guid walletId,
        CancellationToken cancellationToken = default);
}