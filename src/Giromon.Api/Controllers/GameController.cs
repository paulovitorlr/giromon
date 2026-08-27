using System.Security.Claims;
using Giromon.Api.Contracts.Games;
using Giromon.Application.Games.PlaySlot;
using Giromon.Application.Wallets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Giromon.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/games")]
public sealed class GameController : ControllerBase
{
    private readonly PlaySlotUseCase _playSlotUseCase;

    public GameController(PlaySlotUseCase playSlotUseCase)
    {
        _playSlotUseCase = playSlotUseCase;
    }

    [HttpPost("slot/play")]
    public async Task<IActionResult> PlaySlot(
        PlaySlotRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetAuthenticatedUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            var result = await _playSlotUseCase.ExecuteAsync(
                new PlaySlotCommand(
                    userId.Value,
                    request.BetAmount),
                cancellationToken);

            return Ok(new PlaySlotResponse(
                result.RoundId,
                result.FirstSymbol,
                result.SecondSymbol,
                result.ThirdSymbol,
                result.BetAmount,
                result.PrizeAmount,
                result.Balance,
                result.CreatedAt));
        }
        catch (WalletNotFoundException exception)
        {
            return NotFound(new
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
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }

    private Guid? GetAuthenticatedUserId()
    {
        var value = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        return Guid.TryParse(value, out var userId)
            ? userId
            : null;
    }
}