using Giromon.Application.Abstractions.Persistence;
using Giromon.Application.Wallets;
using Giromon.Application.Wallets.Deposit;
using Giromon.Domain.Entities;

namespace Giromon.Application.Tests.Wallets.Deposit;

public class DepositUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldDepositAndSaveTransaction()
    {
        var userId = Guid.NewGuid();
        var wallet = Wallet.Create(userId);

        var walletRepository = new FakeWalletRepository(wallet);
        var transactionRepository =
            new FakeWalletTransactionRepository();
        var unitOfWork = new FakeUnitOfWork();

        var useCase = new DepositUseCase(
            walletRepository,
            transactionRepository,
            unitOfWork);

        var result = await useCase.ExecuteAsync(
            new DepositCommand(userId, 100m));

        var transaction =
            Assert.Single(transactionRepository.Transactions);

        Assert.Equal(100m, wallet.Balance);
        Assert.Equal(100m, result.Balance);
        Assert.Equal(transaction.Id, result.TransactionId);
        Assert.Equal(100m, result.Amount);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenWalletDoesNotExist()
    {
        var walletRepository = new FakeWalletRepository();
        var transactionRepository =
            new FakeWalletTransactionRepository();
        var unitOfWork = new FakeUnitOfWork();

        var useCase = new DepositUseCase(
            walletRepository,
            transactionRepository,
            unitOfWork);

        await Assert.ThrowsAsync<WalletNotFoundException>(
            () => useCase.ExecuteAsync(
                new DepositCommand(Guid.NewGuid(), 100m)));

        Assert.Empty(transactionRepository.Transactions);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenAmountIsNotPositive()
    {
        var userId = Guid.NewGuid();
        var wallet = Wallet.Create(userId);

        var walletRepository = new FakeWalletRepository(wallet);
        var transactionRepository =
            new FakeWalletTransactionRepository();
        var unitOfWork = new FakeUnitOfWork();

        var useCase = new DepositUseCase(
            walletRepository,
            transactionRepository,
            unitOfWork);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => useCase.ExecuteAsync(
                new DepositCommand(userId, 0m)));

        Assert.Equal(0m, wallet.Balance);
        Assert.Empty(transactionRepository.Transactions);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
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

    private sealed class FakeWalletTransactionRepository
        : IWalletTransactionRepository
    {
        public List<WalletTransaction> Transactions { get; } = [];

        public Task AddAsync(
            WalletTransaction transaction,
            CancellationToken cancellationToken = default)
        {
            Transactions.Add(transaction);

            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<WalletTransaction>>
            GetByWalletIdAsync(
                Guid walletId,
                CancellationToken cancellationToken = default)
        {
            IReadOnlyList<WalletTransaction> transactions =
                Transactions
                    .Where(transaction =>
                        transaction.WalletId == walletId)
                    .ToList();

            return Task.FromResult(transactions);
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCallCount { get; private set; }

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;

            return Task.FromResult(1);
        }
    }
}