namespace Giromon.Domain.Entities;

public class Wallet
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public decimal Balance { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Wallet()
    {
    }

    private Wallet(
        Guid id,
        Guid userId,
        decimal balance,
        DateTime createdAt)
    {
        Id = id;
        UserId = userId;
        Balance = balance;
        CreatedAt = createdAt;
    }

    public static Wallet Create(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "O identificador do usuário é obrigatório.",
                nameof(userId));
        }

        return new Wallet(
            Guid.NewGuid(),
            userId,
            0m,
            DateTime.UtcNow);
    }

    public WalletTransaction Deposit(decimal amount)
    {
        var transaction = WalletTransaction.CreateDeposit(
            Id,
            amount);

        Balance += amount;

        return transaction;
    }

    public WalletTransaction Bet(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "O valor da aposta deve ser maior que zero.");
        }

        if (amount > Balance)
        {
            throw new InvalidOperationException(
                "Saldo insuficiente para realizar a aposta.");
        }

        var transaction = WalletTransaction.CreateBet(
            Id,
            amount);

        Balance -= amount;

        return transaction;
    }

    public WalletTransaction CreditPrize(decimal amount)
    {
        var transaction = WalletTransaction.CreatePrize(
            Id,
            amount);

        Balance += amount;

        return transaction;
    }
}