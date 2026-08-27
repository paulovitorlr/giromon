using Giromon.Application.Abstractions.Persistence;
using Giromon.Application.Wallets;
using Giromon.Application.Wallets.GetWallet;
using Giromon.Domain.Entities;

namespace Giromon.Application.Tests.Wallets.GetWallet;

public class GetWalletUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturnWallet()
    {
        var userId = Guid.NewGuid();
        var wallet = Wallet.Create(userId);
        wallet.Deposit(150m);

        var repository = new FakeWalletRepository(wallet);
        var useCase = new GetWalletUseCase(repository);

        var result = await useCase.ExecuteAsync(
            new GetWalletQuery(userId));

        Assert.Equal(wallet.Id, result.Id);
        Assert.Equal(150m, result.Balance);
        Assert.Equal(wallet.CreatedAt, result.CreatedAt);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenWalletDoesNotExist()
    {
        var repository = new FakeWalletRepository();
        var useCase = new GetWalletUseCase(repository);

        await Assert.ThrowsAsync<WalletNotFoundException>(
            () => useCase.ExecuteAsync(
                new GetWalletQuery(Guid.NewGuid())));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenUserIdIsEmpty()
    {
        var repository = new FakeWalletRepository();
        var useCase = new GetWalletUseCase(repository);

        await Assert.ThrowsAsync<ArgumentException>(
            () => useCase.ExecuteAsync(
                new GetWalletQuery(Guid.Empty)));
    }

    private sealed class FakeWalletRepository : IWalletRepository
    {
        private readonly Wallet? _wallet;

        public FakeWalletRepository(Wallet? wallet = null)
        {
            _wallet = wallet;
        }

        public Task<Wallet?> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var wallet = _wallet?.UserId == userId
                ? _wallet
                : null;

            return Task.FromResult(wallet);
        }

        public Task AddAsync(
            Wallet wallet,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}