using Giromon.Application.Abstractions.Persistence;

namespace Giromon.Application.Wallets.Deposit;

public sealed class DepositUseCase
{
    private readonly IWalletRepository _walletRepository;
    private readonly IWalletTransactionRepository
        _walletTransactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DepositUseCase(
        IWalletRepository walletRepository,
        IWalletTransactionRepository walletTransactionRepository,
        IUnitOfWork unitOfWork)
    {
        _walletRepository = walletRepository;
        _walletTransactionRepository = walletTransactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DepositResult> ExecuteAsync(
        DepositCommand command,
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

        var transaction = wallet.Deposit(command.Amount);

        await _walletTransactionRepository.AddAsync(
            transaction,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DepositResult(
            transaction.Id,
            transaction.Type,
            transaction.Amount,
            wallet.Balance,
            transaction.CreatedAt);
    }
}