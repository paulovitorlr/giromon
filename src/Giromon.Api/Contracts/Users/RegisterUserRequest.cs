namespace Giromon.Api.Contracts.Users;

public sealed record RegisterUserRequest(
    string Name,
    string Email,
    string Password);