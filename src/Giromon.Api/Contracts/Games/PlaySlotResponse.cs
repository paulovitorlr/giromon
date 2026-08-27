using Giromon.Domain.Enums;

namespace Giromon.Api.Contracts.Games;

public sealed record PlaySlotResponse(
    Guid RoundId,
    SlotSymbol FirstSymbol,
    SlotSymbol SecondSymbol,
    SlotSymbol ThirdSymbol,
    decimal BetAmount,
    decimal PrizeAmount,
    decimal Balance,
    DateTime CreatedAt);