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
}