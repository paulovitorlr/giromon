using Giromon.Domain.Enums;

namespace Giromon.Domain.Services;

public static class PrizeCalculator
{
    public static decimal Calculate(
        decimal betAmount,
        SlotSymbol first,
        SlotSymbol second,
        SlotSymbol third)
    {
        if (first != second || second != third)
        {
            return 0m;
        }

        var multiplier = first switch
        {
            SlotSymbol.Leaf => 2m,
            SlotSymbol.Water => 3m,
            SlotSymbol.Fire => 5m,
            SlotSymbol.Lightning => 10m,
            SlotSymbol.Master => 20m,
            _ => throw new ArgumentOutOfRangeException(
                nameof(first),
                "Símbolo inválido.")
        };

        return betAmount * multiplier;
    }
}