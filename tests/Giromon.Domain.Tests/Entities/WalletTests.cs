using Giromon.Domain.Entities;

namespace Giromon.Domain.Tests.Entities;

public class WalletTests
{
    [Fact]
    public void Create_ShouldCreateWalletWithZeroBalance()
    {
        var userId = Guid.NewGuid();

        var wallet = Wallet.Create(userId);

        Assert.NotEqual(Guid.Empty, wallet.Id);
        Assert.Equal(userId, wallet.UserId);
        Assert.Equal(0m, wallet.Balance);
        Assert.True(wallet.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Create_ShouldThrow_WhenUserIdIsEmpty()
    {
        var action = () => Wallet.Create(Guid.Empty);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Deposit_ShouldIncreaseBalanceAndCreateTransaction()
    {
        var wallet = Wallet.Create(Guid.NewGuid());

        var transaction = wallet.Deposit(100m);

        Assert.Equal(100m, wallet.Balance);
        Assert.Equal(wallet.Id, transaction.WalletId);
        Assert.Equal(100m, transaction.Amount);
        Assert.Equal(
            Giromon.Domain.Enums.WalletTransactionType.Deposit,
            transaction.Type);
    }

    [Fact]
    public void Deposit_ShouldAccumulateBalance()
    {
        var wallet = Wallet.Create(Guid.NewGuid());

        wallet.Deposit(100m);
        wallet.Deposit(50m);

        Assert.Equal(150m, wallet.Balance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Deposit_ShouldThrowAndPreserveBalance_WhenAmountIsNotPositive(
        decimal amount)
    {
        var wallet = Wallet.Create(Guid.NewGuid());

        var action = () => wallet.Deposit(amount);

        Assert.Throws<ArgumentOutOfRangeException>(action);
        Assert.Equal(0m, wallet.Balance);
    }
}