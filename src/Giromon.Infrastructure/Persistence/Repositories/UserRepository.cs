using Giromon.Application.Abstractions.Persistence;
using Giromon.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Giromon.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly GiromonDbContext _dbContext;

    public UserRepository(GiromonDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExistsByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AnyAsync(
                user => user.Email == email,
                cancellationToken);
    }

    public async Task AddAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(
            user,
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}