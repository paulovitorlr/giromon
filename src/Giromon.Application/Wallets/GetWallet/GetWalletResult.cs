namespace Giromon.Application.Wallets.GetWallet;

public sealed record GetWalletResult(
    Guid Id,
    decimal Balance,
    DateTime CreatedAt);