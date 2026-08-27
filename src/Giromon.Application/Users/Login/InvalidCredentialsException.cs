namespace Giromon.Application.Users.Login;

public sealed class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException()
        : base("E-mail ou senha inválidos.")
    {
    }
}