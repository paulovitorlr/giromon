using Giromon.Domain.Enums;
using Giromon.Domain.Services;

namespace Giromon.Domain.Tests.Services;

public class PrizeCalculatorTests
{
    [Theory]
    [InlineData(SlotSymbol.Leaf, 2)]
    [InlineData(SlotSymbol.Water, 3)]
    [InlineData(SlotSymbol.Fire, 5)]
    [InlineData(SlotSymbol.Lightning, 10)]
    [InlineData(SlotSymbol.Master, 20)]
    public void Calculate_ShouldReturnPrize_WhenAllSymbolsAreEqual(
        SlotSymbol symbol,
        int multiplier)
    {
        const decimal betAmount = 0.50m;

        var prize = PrizeCalculator.Calculate(
            betAmount,
            symbol,
            symbol,
            symbol);

        Assert.Equal(betAmount * multiplier, prize);
    }

    [Theory]
    [InlineData(SlotSymbol.Leaf, SlotSymbol.Leaf, SlotSymbol.Water)]
    [InlineData(SlotSymbol.Fire, SlotSymbol.Water, SlotSymbol.Fire)]
    [InlineData(SlotSymbol.Master, SlotSymbol.Leaf, SlotSymbol.Master)]
    public void Calculate_ShouldReturnZero_WhenSymbolsAreDifferent(
        SlotSymbol first,
        SlotSymbol second,
        SlotSymbol third)
    {
        var prize = PrizeCalculator.Calculate(
            10m,
            first,
            second,
            third);

        Assert.Equal(0m, prize);
    }
}