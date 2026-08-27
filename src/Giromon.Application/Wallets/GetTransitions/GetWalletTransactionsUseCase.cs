using Giromon.Application.Abstractions.Persistence;

namespace Giromon.Application.Wallets.GetTransactions;

public sealed class GetWalletTransactionsUseCase
{
    private readonly IWalletRepository _walletRepository;
    private readonly IWalletTransactionRepository
        _walletTransactionRepository;

    public GetWalletTransactionsUseCase(
        IWalletRepository walletRepository,
        IWalletTransactionRepository walletTransactionRepository)
    {
        _walletRepository = walletRepository;
        _walletTransactionRepository = walletTransactionRepository;
    }

    public async Task<IReadOnlyList<WalletTransactionResult>> ExecuteAsync(
        GetWalletTransactionsQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.UserId == Guid.Empty)
        {
            throw new ArgumentException(
                "O identificador do usuário é obrigatório.",
                nameof(query.UserId));
        }

        var wallet = await _walletRepository.GetByUserIdAsync(
            query.UserId,
            cancellationToken);

        if (wallet is null)
        {
            throw new WalletNotFoundException();
        }

        var transactions =
            await _walletTransactionRepository.GetByWalletIdAsync(
                wallet.Id,
                cancellationToken);

        return transactions
            .Select(transaction => new WalletTransactionResult(
                transaction.Id,
                transaction.Type,
                transaction.Amount,
                transaction.CreatedAt))
            .ToList();
    }
}