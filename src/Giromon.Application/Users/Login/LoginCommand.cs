namespace Giromon.Application.Users.Login;

public sealed record LoginCommand(
    string Email,
    string Password);