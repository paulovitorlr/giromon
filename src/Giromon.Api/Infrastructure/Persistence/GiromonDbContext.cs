using Microsoft.EntityFrameworkCore;

namespace Giromon.Api.Infrastructure.Persistence;

public sealed class GiromonDbContext(DbContextOptions<GiromonDbContext> options)
    : DbContext(options)
{
}

