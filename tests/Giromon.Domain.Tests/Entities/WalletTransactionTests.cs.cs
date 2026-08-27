using Giromon.Domain.Entities;
using Giromon.Domain.Enums;

namespace Giromon.Domain.Tests.Entities;

public class WalletTransactionTests
{
    [Fact]
    public void CreateDeposit_ShouldCreateDepositTransaction()
    {
        var walletId = Guid.NewGuid();
        const decimal amount = 100m;

        var transaction = WalletTransaction.CreateDeposit(
            walletId,
            amount);

        Assert.NotEqual(Guid.Empty, transaction.Id);
        Assert.Equal(walletId, transaction.WalletId);
        Assert.Equal(WalletTransactionType.Deposit, transaction.Type);
        Assert.Equal(amount, transaction.Amount);
        Assert.True(transaction.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void CreateDeposit_ShouldThrow_WhenWalletIdIsEmpty()
    {
        var action = () =>
            WalletTransaction.CreateDeposit(Guid.Empty, 100m);

        Assert.Throws<ArgumentException>(action);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void CreateDeposit_ShouldThrow_WhenAmountIsNotPositive(
        decimal amount)
    {
        var action = () =>
            WalletTransaction.CreateDeposit(
                Guid.NewGuid(),
                amount);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }
}