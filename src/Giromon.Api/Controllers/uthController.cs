using Giromon.Api.Contracts.Auth;
using Giromon.Application.Users.Login;
using Microsoft.AspNetCore.Mvc;

namespace Giromon.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly LoginUseCase _loginUseCase;

    public AuthController(LoginUseCase loginUseCase)
    {
        _loginUseCase = loginUseCase;
    }

    [HttpPost("login")]
    [ProducesResponseType<LoginResponse>(
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new LoginCommand(
                request.Email,
                request.Password);

            var result = await _loginUseCase.ExecuteAsync(
                command,
                cancellationToken);

            var response = new LoginResponse(
                result.UserId,
                result.Name,
                result.Email,
                result.AccessToken);

            return Ok(response);
        }
        catch (InvalidCredentialsException exception)
        {
            return Unauthorized(new
            {
                message = exception.Message
            });
        }
    }
}