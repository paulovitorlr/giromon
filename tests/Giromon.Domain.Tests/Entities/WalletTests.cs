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
}