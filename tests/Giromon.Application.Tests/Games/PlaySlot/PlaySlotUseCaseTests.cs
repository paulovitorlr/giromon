using Giromon.Application.Abstractions.Games;
using Giromon.Application.Abstractions.Persistence;
using Giromon.Application.Games.PlaySlot;
using Giromon.Domain.Entities;
using Giromon.Domain.Enums;
using Giromon.Application.Wallets;

namespace Giromon.Application.Tests.Games.PlaySlot;

public class PlaySlotUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldPlayRoundWithoutPrize()
    {
        var userId = Guid.NewGuid();
        var wallet = Wallet.Create(userId);
        wallet.Deposit(100m);

        var walletRepository = new FakeWalletRepository(wallet);
        var transactionRepository =
            new FakeWalletTransactionRepository();
        var gameRoundRepository = new FakeGameRoundRepository();
        var symbolGenerator = new FakeSlotSymbolGenerator(
            SlotSymbol.Leaf,
            SlotSymbol.Water,
            SlotSymbol.Fire);
        var unitOfWork = new FakeUnitOfWork();

        var useCase = new PlaySlotUseCase(
            walletRepository,
            transactionRepository,
            gameRoundRepository,
            symbolGenerator,
            unitOfWork);

        var result = await useCase.ExecuteAsync(
            new PlaySlotCommand(userId, 10m));

        var transaction =
            Assert.Single(transactionRepository.Transactions);
        var gameRound =
            Assert.Single(gameRoundRepository.GameRounds);

        Assert.Equal(WalletTransactionType.Bet, transaction.Type);
        Assert.Equal(10m, transaction.Amount);

        Assert.Equal(SlotSymbol.Leaf, result.FirstSymbol);
        Assert.Equal(SlotSymbol.Water, result.SecondSymbol);
        Assert.Equal(SlotSymbol.Fire, result.ThirdSymbol);
        Assert.Equal(10m, result.BetAmount);
        Assert.Equal(0m, result.PrizeAmount);
        Assert.Equal(90m, result.Balance);

        Assert.Equal(gameRound.Id, result.RoundId);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
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
                Transactions;

            return Task.FromResult(transactions);
        }
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCreditPrize_WhenRoundWins()
    {
        var userId = Guid.NewGuid();
        var wallet = Wallet.Create(userId);
        wallet.Deposit(100m);

        var walletRepository = new FakeWalletRepository(wallet);
        var transactionRepository =
            new FakeWalletTransactionRepository();
        var gameRoundRepository = new FakeGameRoundRepository();
        var symbolGenerator = new FakeSlotSymbolGenerator(
            SlotSymbol.Master,
            SlotSymbol.Master,
            SlotSymbol.Master);
        var unitOfWork = new FakeUnitOfWork();

        var useCase = new PlaySlotUseCase(
            walletRepository,
            transactionRepository,
            gameRoundRepository,
            symbolGenerator,
            unitOfWork);

        var result = await useCase.ExecuteAsync(
            new PlaySlotCommand(userId, 10m));

        Assert.Equal(2, transactionRepository.Transactions.Count);

        var betTransaction = transactionRepository.Transactions[0];
        var prizeTransaction = transactionRepository.Transactions[1];

        Assert.Equal(
            WalletTransactionType.Bet,
            betTransaction.Type);
        Assert.Equal(10m, betTransaction.Amount);

        Assert.Equal(
            WalletTransactionType.Prize,
            prizeTransaction.Type);
        Assert.Equal(200m, prizeTransaction.Amount);

        Assert.Equal(200m, result.PrizeAmount);
        Assert.Equal(290m, result.Balance);
        Assert.Equal(290m, wallet.Balance);

        Assert.Single(gameRoundRepository.GameRounds);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenBalanceIsInsufficient()
    {
        var userId = Guid.NewGuid();
        var wallet = Wallet.Create(userId);
        wallet.Deposit(5m);

        var walletRepository = new FakeWalletRepository(wallet);
        var transactionRepository =
            new FakeWalletTransactionRepository();
        var gameRoundRepository = new FakeGameRoundRepository();
        var symbolGenerator = new FakeSlotSymbolGenerator(
            SlotSymbol.Leaf,
            SlotSymbol.Leaf,
            SlotSymbol.Leaf);
        var unitOfWork = new FakeUnitOfWork();

        var useCase = new PlaySlotUseCase(
            walletRepository,
            transactionRepository,
            gameRoundRepository,
            symbolGenerator,
            unitOfWork);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => useCase.ExecuteAsync(
                new PlaySlotCommand(userId, 10m)));

        Assert.Equal(5m, wallet.Balance);
        Assert.Empty(transactionRepository.Transactions);
        Assert.Empty(gameRoundRepository.GameRounds);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenWalletDoesNotExist()
    {
        var walletRepository = new FakeWalletRepository();
        var transactionRepository =
            new FakeWalletTransactionRepository();
        var gameRoundRepository = new FakeGameRoundRepository();
        var symbolGenerator = new FakeSlotSymbolGenerator(
            SlotSymbol.Leaf,
            SlotSymbol.Water,
            SlotSymbol.Fire);
        var unitOfWork = new FakeUnitOfWork();

        var useCase = new PlaySlotUseCase(
            walletRepository,
            transactionRepository,
            gameRoundRepository,
            symbolGenerator,
            unitOfWork);

        await Assert.ThrowsAsync<WalletNotFoundException>(
            () => useCase.ExecuteAsync(
                new PlaySlotCommand(Guid.NewGuid(), 10m)));

        Assert.Empty(transactionRepository.Transactions);
        Assert.Empty(gameRoundRepository.GameRounds);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenBetIsBelowMinimum()
    {
        var userId = Guid.NewGuid();
        var wallet = Wallet.Create(userId);
        wallet.Deposit(100m);

        var walletRepository = new FakeWalletRepository(wallet);
        var transactionRepository =
            new FakeWalletTransactionRepository();
        var gameRoundRepository = new FakeGameRoundRepository();
        var symbolGenerator = new FakeSlotSymbolGenerator(
            SlotSymbol.Leaf,
            SlotSymbol.Leaf,
            SlotSymbol.Leaf);
        var unitOfWork = new FakeUnitOfWork();

        var useCase = new PlaySlotUseCase(
            walletRepository,
            transactionRepository,
            gameRoundRepository,
            symbolGenerator,
            unitOfWork);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => useCase.ExecuteAsync(
                new PlaySlotCommand(userId, 0.49m)));

        Assert.Equal(100m, wallet.Balance);
        Assert.Empty(transactionRepository.Transactions);
        Assert.Empty(gameRoundRepository.GameRounds);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenUserIdIsEmpty()
    {
        var walletRepository = new FakeWalletRepository();
        var transactionRepository =
            new FakeWalletTransactionRepository();
        var gameRoundRepository = new FakeGameRoundRepository();
        var symbolGenerator = new FakeSlotSymbolGenerator(
            SlotSymbol.Leaf,
            SlotSymbol.Water,
            SlotSymbol.Fire);
        var unitOfWork = new FakeUnitOfWork();

        var useCase = new PlaySlotUseCase(
            walletRepository,
            transactionRepository,
            gameRoundRepository,
            symbolGenerator,
            unitOfWork);

        await Assert.ThrowsAsync<ArgumentException>(
            () => useCase.ExecuteAsync(
                new PlaySlotCommand(Guid.Empty, 10m)));

        Assert.Empty(transactionRepository.Transactions);
        Assert.Empty(gameRoundRepository.GameRounds);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    private sealed class FakeGameRoundRepository
        : IGameRoundRepository
    {
        public List<GameRound> GameRounds { get; } = [];

        public Task AddAsync(
            GameRound gameRound,
            CancellationToken cancellationToken = default)
        {
            GameRounds.Add(gameRound);

            return Task.CompletedTask;
        }
    }

    private sealed class FakeSlotSymbolGenerator
        : ISlotSymbolGenerator
    {
        private readonly Queue<SlotSymbol> _symbols;

        public FakeSlotSymbolGenerator(params SlotSymbol[] symbols)
        {
            _symbols = new Queue<SlotSymbol>(symbols);
        }

        public SlotSymbol Generate()
        {
            return _symbols.Dequeue();
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