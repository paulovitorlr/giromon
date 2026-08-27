using Giromon.Domain.Enums;

namespace Giromon.Application.Games.PlaySlot;

public sealed record PlaySlotResult(
    Guid RoundId,
    SlotSymbol FirstSymbol,
    SlotSymbol SecondSymbol,
    SlotSymbol ThirdSymbol,
    decimal BetAmount,
    decimal PrizeAmount,
    decimal Balance,
    DateTime CreatedAt);