using Giromon.Domain.Entities;

namespace Giromon.Application.Abstractions.Persistence;

public interface IWalletRepository
{
    Task<Wallet?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Wallet wallet,
        CancellationToken cancellationToken = default);
}