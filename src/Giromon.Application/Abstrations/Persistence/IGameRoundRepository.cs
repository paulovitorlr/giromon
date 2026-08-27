using Giromon.Domain.Entities;

namespace Giromon.Application.Abstractions.Persistence;

public interface IGameRoundRepository
{
    Task AddAsync(
        GameRound gameRound,
        CancellationToken cancellationToken = default);
}