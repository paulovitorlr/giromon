using Giromon.Api.Contracts.Users;
using Giromon.Application.Users.Register;
using Microsoft.AspNetCore.Mvc;

namespace Giromon.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly RegisterUserUseCase _registerUserUseCase;

    public UsersController(RegisterUserUseCase registerUserUseCase)
    {
        _registerUserUseCase = registerUserUseCase;
    }

    [HttpPost("register")]
    [ProducesResponseType<RegisterUserResponse>(
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new RegisterUserCommand(
                request.Name,
                request.Email,
                request.Password);

            var result = await _registerUserUseCase.ExecuteAsync(
                command,
                cancellationToken);

            var response = new RegisterUserResponse(
                result.Id,
                result.Name,
                result.Email,
                result.CreatedAt);

            return StatusCode(
                StatusCodes.Status201Created,
                response);
        }
        catch (EmailAlreadyInUseException exception)
        {
            return Conflict(new
            {
                message = exception.Message
            });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }
}