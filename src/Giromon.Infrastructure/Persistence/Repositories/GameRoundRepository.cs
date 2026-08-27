using Giromon.Application.Abstractions.Persistence;
using Giromon.Domain.Entities;

namespace Giromon.Infrastructure.Persistence.Repositories;

public class GameRoundRepository : IGameRoundRepository
{
    private readonly GiromonDbContext _dbContext;

    public GameRoundRepository(GiromonDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
     GameRound gameRound,
     CancellationToken cancellationToken = default)
    {
        await _dbContext.GameRounds.AddAsync(
            gameRound,
            cancellationToken);
    }
}