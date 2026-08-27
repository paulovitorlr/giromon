namespace Giromon.Api.Contracts.Wallets;

public sealed record WalletResponse(
    Guid Id,
    decimal Balance,
    DateTime CreatedAt);