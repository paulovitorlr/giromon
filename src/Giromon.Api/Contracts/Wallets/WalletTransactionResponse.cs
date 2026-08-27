using Giromon.Domain.Enums;

namespace Giromon.Api.Contracts.Wallets;

public sealed record WalletTransactionResponse(
    Guid Id,
    WalletTransactionType Type,
    decimal Amount,
    DateTime CreatedAt);