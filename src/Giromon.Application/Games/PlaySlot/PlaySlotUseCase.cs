using Giromon.Application.Abstractions.Games;
using Giromon.Application.Abstractions.Persistence;
using Giromon.Application.Wallets;
using Giromon.Domain.Entities;

namespace Giromon.Application.Games.PlaySlot;

public sealed class PlaySlotUseCase
{
    private readonly IWalletRepository _walletRepository;
    private readonly IWalletTransactionRepository
        _walletTransactionRepository;
    private readonly IGameRoundRepository _gameRoundRepository;
    private readonly ISlotSymbolGenerator _symbolGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public PlaySlotUseCase(
        IWalletRepository walletRepository,
        IWalletTransactionRepository walletTransactionRepository,
        IGameRoundRepository gameRoundRepository,
        ISlotSymbolGenerator symbolGenerator,
        IUnitOfWork unitOfWork)
    {
        _walletRepository = walletRepository;
        _walletTransactionRepository = walletTransactionRepository;
        _gameRoundRepository = gameRoundRepository;
        _symbolGenerator = symbolGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<PlaySlotResult> ExecuteAsync(
        PlaySlotCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.UserId == Guid.Empty)
        {
            throw new ArgumentException(
                "O identificador do usuário é obrigatório.",
                nameof(command.UserId));
        }

        var wallet = await _walletRepository.GetByUserIdAsync(
            command.UserId,
            cancellationToken);

        if (wallet is null)
        {
            throw new WalletNotFoundException();
        }

        var firstSymbol = _symbolGenerator.Generate();
        var secondSymbol = _symbolGenerator.Generate();
        var thirdSymbol = _symbolGenerator.Generate();

        var gameRound = GameRound.Create(
            command.UserId,
            command.BetAmount,
            firstSymbol,
            secondSymbol,
            thirdSymbol);

        var betTransaction = wallet.Bet(command.BetAmount);

        await _walletTransactionRepository.AddAsync(
            betTransaction,
            cancellationToken);

        if (gameRound.PrizeAmount > 0)
        {
            var prizeTransaction = wallet.CreditPrize(
                gameRound.PrizeAmount);

            await _walletTransactionRepository.AddAsync(
                prizeTransaction,
                cancellationToken);
        }

        await _gameRoundRepository.AddAsync(
            gameRound,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new PlaySlotResult(
            gameRound.Id,
            gameRound.FirstSymbol,
            gameRound.SecondSymbol,
            gameRound.ThirdSymbol,
            gameRound.BetAmount,
            gameRound.PrizeAmount,
            wallet.Balance,
            gameRound.CreatedAt);
    }
}