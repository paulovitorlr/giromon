using Giromon.Domain.Enums;

namespace Giromon.Domain.Entities;

public class WalletTransaction
{
    public Guid Id { get; private set; }
    public Guid WalletId { get; private set; }
    public WalletTransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private WalletTransaction()
    {
    }

    private WalletTransaction(
        Guid id,
        Guid walletId,
        WalletTransactionType type,
        decimal amount,
        DateTime createdAt)
    {
        Id = id;
        WalletId = walletId;
        Type = type;
        Amount = amount;
        CreatedAt = createdAt;
    }

    public static WalletTransaction CreateDeposit(
        Guid walletId,
        decimal amount)
    {
        if (walletId == Guid.Empty)
        {
            throw new ArgumentException(
                "O identificador da carteira é obrigatório.",
                nameof(walletId));
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "O valor do depósito deve ser maior que zero.");
        }

        return new WalletTransaction(
            Guid.NewGuid(),
            walletId,
            WalletTransactionType.Deposit,
            amount,
            DateTime.UtcNow);
    }

    public static WalletTransaction CreateBet(
    Guid walletId,
    decimal amount)
    {
        if (walletId == Guid.Empty)
        {
            throw new ArgumentException(
                "O identificador da carteira é obrigatório.",
                nameof(walletId));
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "O valor da aposta deve ser maior que zero.");
        }

        return new WalletTransaction(
            Guid.NewGuid(),
            walletId,
            WalletTransactionType.Bet,
            amount,
            DateTime.UtcNow);
    }

    public static WalletTransaction CreatePrize(
    Guid walletId,
    decimal amount)
    {
        if (walletId == Guid.Empty)
        {
            throw new ArgumentException(
                "O identificador da carteira é obrigatório.",
                nameof(walletId));
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "O valor do prêmio deve ser maior que zero.");
        }

        return new WalletTransaction(
            Guid.NewGuid(),
            walletId,
            WalletTransactionType.Prize,
            amount,
            DateTime.UtcNow);
    }
}