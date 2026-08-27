using Giromon.Application.Abstractions.Persistence;
using Giromon.Application.Abstractions.Security;
using Giromon.Infrastructure.Persistence;
using Giromon.Infrastructure.Persistence.Repositories;
using Giromon.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Giromon.Application.Abstractions.Games;
using Giromon.Infrastructure.Games;

namespace Giromon.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        JwtSettings jwtSettings)
    {
        services.AddDbContext<GiromonDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<IWalletTransactionRepository,WalletTransactionRepository>();
        services.AddScoped<IGameRoundRepository, GameRoundRepository>();
        services.AddSingleton<ISlotSymbolGenerator, RandomSlotSymbolGenerator>();

        services.AddScoped<IUnitOfWork>(
            serviceProvider =>
                serviceProvider.GetRequiredService<GiromonDbContext>());

        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

        services.AddSingleton(jwtSettings);

        services.AddScoped<
            IAccessTokenGenerator,
            JwtAccessTokenGenerator>();

        return services;
    }
}