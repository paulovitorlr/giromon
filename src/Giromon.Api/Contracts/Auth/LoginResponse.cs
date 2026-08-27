namespace Giromon.Api.Contracts.Auth;

public sealed record LoginResponse(
    Guid UserId,
    string Name,
    string Email,
    string AccessToken);