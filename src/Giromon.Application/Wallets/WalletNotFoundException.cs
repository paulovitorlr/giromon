namespace Giromon.Application.Wallets;

public sealed class WalletNotFoundException : Exception
{
    public WalletNotFoundException()
        : base("A carteira do usuário não foi encontrada.")
    {
    }
}