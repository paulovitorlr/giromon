using System.Security.Claims;
using Giromon.Api.Contracts.Wallets;
using Giromon.Application.Wallets;
using Giromon.Application.Wallets.Deposit;
using Giromon.Application.Wallets.GetTransactions;
using Giromon.Application.Wallets.GetWallet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Giromon.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/wallet")]
public sealed class WalletController : ControllerBase
{
    private readonly GetWalletUseCase _getWalletUseCase;
    private readonly DepositUseCase _depositUseCase;
    private readonly GetWalletTransactionsUseCase
        _getWalletTransactionsUseCase;

    public WalletController(
        GetWalletUseCase getWalletUseCase,
        DepositUseCase depositUseCase,
        GetWalletTransactionsUseCase getWalletTransactionsUseCase)
    {
        _getWalletUseCase = getWalletUseCase;
        _depositUseCase = depositUseCase;
        _getWalletTransactionsUseCase =
            getWalletTransactionsUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> GetWallet(
        CancellationToken cancellationToken)
    {
        var userId = GetAuthenticatedUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            var result = await _getWalletUseCase.ExecuteAsync(
                new GetWalletQuery(userId.Value),
                cancellationToken);

            return Ok(new WalletResponse(
                result.Id,
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
    }

    [HttpPost("deposits")]
    public async Task<IActionResult> Deposit(
        DepositRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetAuthenticatedUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            var result = await _depositUseCase.ExecuteAsync(
                new DepositCommand(
                    userId.Value,
                    request.Amount),
                cancellationToken);

            return Ok(new DepositResponse(
                result.TransactionId,
                result.Type,
                result.Amount,
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
        catch (ArgumentOutOfRangeException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions(
        CancellationToken cancellationToken)
    {
        var userId = GetAuthenticatedUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            var result =
                await _getWalletTransactionsUseCase.ExecuteAsync(
                    new GetWalletTransactionsQuery(userId.Value),
                    cancellationToken);

            var response = result
                .Select(transaction =>
                    new WalletTransactionResponse(
                        transaction.Id,
                        transaction.Type,
                        transaction.Amount,
                        transaction.CreatedAt))
                .ToList();

            return Ok(response);
        }
        catch (WalletNotFoundException exception)
        {
            return NotFound(new
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