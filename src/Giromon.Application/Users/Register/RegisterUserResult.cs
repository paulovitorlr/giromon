namespace Giromon.Application.Users.Register;

public sealed record RegisterUserResult(
    Guid Id,
    string Name,
    string Email,
    DateTime CreatedAt);