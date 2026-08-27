namespace Giromon.Application.Wallets.Deposit;

public sealed record DepositCommand(
    Guid UserId,
    decimal Amount);