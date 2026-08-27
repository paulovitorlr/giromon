namespace Giromon.Application.Users.Login;

public sealed record LoginResult(
    Guid UserId,
    string Name,
    string Email,
    string AccessToken);