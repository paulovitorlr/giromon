namespace Giromon.Application.Games.PlaySlot;

public sealed record PlaySlotCommand(
    Guid UserId,
    decimal BetAmount);