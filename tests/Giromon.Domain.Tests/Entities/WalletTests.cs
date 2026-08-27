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

    [Fact]
    public void Bet_ShouldDecreaseBalanceAndCreateTransaction()
    {
        var wallet = Wallet.Create(Guid.NewGuid());
        wallet.Deposit(100m);

        var transaction = wallet.Bet(25.50m);

        Assert.Equal(74.50m, wallet.Balance);
        Assert.Equal(wallet.Id, transaction.WalletId);
        Assert.Equal(25.50m, transaction.Amount);
        Assert.Equal(
            Giromon.Domain.Enums.WalletTransactionType.Bet,
            transaction.Type);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.50)]
    [InlineData(-100)]
    public void Bet_ShouldThrowAndPreserveBalance_WhenAmountIsNotPositive(
        decimal amount)
    {
        var wallet = Wallet.Create(Guid.NewGuid());
        wallet.Deposit(100m);

        var action = () => wallet.Bet(amount);

        Assert.Throws<ArgumentOutOfRangeException>(action);
        Assert.Equal(100m, wallet.Balance);
    }

    [Fact]
    public void Bet_ShouldThrowAndPreserveBalance_WhenBalanceIsInsufficient()
    {
        var wallet = Wallet.Create(Guid.NewGuid());
        wallet.Deposit(10m);

        var action = () => wallet.Bet(10.50m);

        Assert.Throws<InvalidOperationException>(action);
        Assert.Equal(10m, wallet.Balance);
    }

    [Fact]
    public void Bet_ShouldAllowUsingEntireBalance()
    {
        var wallet = Wallet.Create(Guid.NewGuid());
        wallet.Deposit(10m);

        var transaction = wallet.Bet(10m);

        Assert.Equal(0m, wallet.Balance);
        Assert.Equal(10m, transaction.Amount);
    }

    [Fact]
    public void CreditPrize_ShouldIncreaseBalanceAndCreateTransaction()
    {
        var wallet = Wallet.Create(Guid.NewGuid());
        wallet.Deposit(100m);

        var transaction = wallet.CreditPrize(75.50m);

        Assert.Equal(175.50m, wallet.Balance);
        Assert.Equal(wallet.Id, transaction.WalletId);
        Assert.Equal(75.50m, transaction.Amount);
        Assert.Equal(
            Giromon.Domain.Enums.WalletTransactionType.Prize,
            transaction.Type);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.50)]
    [InlineData(-100)]
    public void CreditPrize_ShouldThrowAndPreserveBalance_WhenAmountIsNotPositive(
        decimal amount)
    {
        var wallet = Wallet.Create(Guid.NewGuid());
        wallet.Deposit(100m);

        var action = () => wallet.CreditPrize(amount);

        Assert.Throws<ArgumentOutOfRangeException>(action);
        Assert.Equal(100m, wallet.Balance);
    }
}