using Giromon.Application.Abstractions.Persistence;

namespace Giromon.Application.Wallets.GetWallet;

public sealed class GetWalletUseCase
{
    private readonly IWalletRepository _walletRepository;

    public GetWalletUseCase(
        IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task<GetWalletResult> ExecuteAsync(
        GetWalletQuery query,
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

        return new GetWalletResult(
            wallet.Id,
            wallet.Balance,
            wallet.CreatedAt);
    }
}