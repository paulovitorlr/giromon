using Giromon.Domain.Enums;

namespace Giromon.Application.Wallets.GetTransactions;

public sealed record WalletTransactionResult(
    Guid Id,
    WalletTransactionType Type,
    decimal Amount,
    DateTime CreatedAt);