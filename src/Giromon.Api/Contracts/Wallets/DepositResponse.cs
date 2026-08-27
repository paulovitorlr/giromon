using Giromon.Domain.Enums;

namespace Giromon.Api.Contracts.Wallets;

public sealed record DepositResponse(
    Guid TransactionId,
    WalletTransactionType Type,
    decimal Amount,
    decimal Balance,
    DateTime CreatedAt);