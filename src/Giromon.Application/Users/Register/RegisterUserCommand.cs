namespace Giromon.Application.Users.Register;

public sealed record RegisterUserCommand(
    string Name,
    string Email,
    string Password);