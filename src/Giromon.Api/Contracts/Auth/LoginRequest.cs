namespace Giromon.Api.Contracts.Auth;

public sealed record LoginRequest(
    string Email,
    string Password);