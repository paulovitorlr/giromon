using Giromon.Application.Abstractions.Persistence;
using Giromon.Application.Wallets;
using Giromon.Application.Wallets.GetTransactions;
using Giromon.Domain.Entities;

namespace Giromon.Application.Tests.Wallets.GetTransactions;

public class GetWalletTransactionsUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturnWalletTransactions()
    {
        var userId = Guid.NewGuid();
        var wallet = Wallet.Create(userId);

        var firstTransaction = wallet.Deposit(100m);
        var secondTransaction = wallet.Deposit(50m);

        var walletRepository = new FakeWalletRepository(wallet);
        var transactionRepository =
            new FakeWalletTransactionRepository(
                firstTransaction,
                secondTransaction);

        var useCase = new GetWalletTransactionsUseCase(
            walletRepository,
            transactionRepository);

        var result = await useCase.ExecuteAsync(
            new GetWalletTransactionsQuery(userId));

        Assert.Equal(2, result.Count);

        Assert.Contains(
            result,
            transaction => transaction.Id == firstTransaction.Id);

        Assert.Contains(
            result,
            transaction => transaction.Id == secondTransaction.Id);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnEmptyList_WhenThereAreNoTransactions()
    {
        var userId = Guid.NewGuid();
        var wallet = Wallet.Create(userId);

        var walletRepository = new FakeWalletRepository(wallet);
        var transactionRepository =
            new FakeWalletTransactionRepository();

        var useCase = new GetWalletTransactionsUseCase(
            walletRepository,
            transactionRepository);

        var result = await useCase.ExecuteAsync(
            new GetWalletTransactionsQuery(userId));

        Assert.Empty(result);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenWalletDoesNotExist()
    {
        var walletRepository = new FakeWalletRepository();
        var transactionRepository =
            new FakeWalletTransactionRepository();

        var useCase = new GetWalletTransactionsUseCase(
            walletRepository,
            transactionRepository);

        await Assert.ThrowsAsync<WalletNotFoundException>(
            () => useCase.ExecuteAsync(
                new GetWalletTransactionsQuery(Guid.NewGuid())));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenUserIdIsEmpty()
    {
        var walletRepository = new FakeWalletRepository();
        var transactionRepository =
            new FakeWalletTransactionRepository();

        var useCase = new GetWalletTransactionsUseCase(
            walletRepository,
            transactionRepository);

        await Assert.ThrowsAsync<ArgumentException>(
            () => useCase.ExecuteAsync(
                new GetWalletTransactionsQuery(Guid.Empty)));
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
        private readonly List<WalletTransaction> _transactions;

        public FakeWalletTransactionRepository(
            params WalletTransaction[] transactions)
        {
            _transactions = transactions.ToList();
        }

        public Task AddAsync(
            WalletTransaction transaction,
            CancellationToken cancellationToken = default)
        {
            _transactions.Add(transaction);

            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<WalletTransaction>>
            GetByWalletIdAsync(
                Guid walletId,
                CancellationToken cancellationToken = default)
        {
            IReadOnlyList<WalletTransaction> transactions =
                _transactions
                    .Where(transaction =>
                        transaction.WalletId == walletId)
                    .OrderByDescending(transaction =>
                        transaction.CreatedAt)
                    .ToList();

            return Task.FromResult(transactions);
        }
    }
}