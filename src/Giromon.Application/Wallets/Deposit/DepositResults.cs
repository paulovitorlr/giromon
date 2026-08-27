using Giromon.Domain.Enums;

namespace Giromon.Application.Wallets.Deposit;

public sealed record DepositResult(
    Guid TransactionId,
    WalletTransactionType Type,
    decimal Amount,
    decimal Balance,
    DateTime CreatedAt);