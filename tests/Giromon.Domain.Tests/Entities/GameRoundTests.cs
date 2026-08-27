using Giromon.Domain.Entities;
using Giromon.Domain.Enums;

namespace Giromon.Domain.Tests.Entities;

public class GameRoundTests
{
    [Fact]
    public void Create_ShouldCreateWinningRoundAndCalculatePrize()
    {
        var userId = Guid.NewGuid();

        var round = GameRound.Create(
            userId,
            10m,
            SlotSymbol.Fire,
            SlotSymbol.Fire,
            SlotSymbol.Fire);

        Assert.NotEqual(Guid.Empty, round.Id);
        Assert.Equal(userId, round.UserId);
        Assert.Equal(10m, round.BetAmount);
        Assert.Equal(SlotSymbol.Fire, round.FirstSymbol);
        Assert.Equal(SlotSymbol.Fire, round.SecondSymbol);
        Assert.Equal(SlotSymbol.Fire, round.ThirdSymbol);
        Assert.Equal(50m, round.PrizeAmount);
        Assert.True(round.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Create_ShouldCreateLosingRoundWithZeroPrize()
    {
        var round = GameRound.Create(
            Guid.NewGuid(),
            10m,
            SlotSymbol.Leaf,
            SlotSymbol.Fire,
            SlotSymbol.Water);

        Assert.Equal(0m, round.PrizeAmount);
    }

    [Fact]
    public void Create_ShouldAllowMinimumBet()
    {
        var round = GameRound.Create(
            Guid.NewGuid(),
            0.50m,
            SlotSymbol.Leaf,
            SlotSymbol.Water,
            SlotSymbol.Fire);

        Assert.Equal(0.50m, round.BetAmount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(0.49)]
    [InlineData(-1)]
    public void Create_ShouldThrow_WhenBetIsBelowMinimum(
        decimal betAmount)
    {
        var action = () => GameRound.Create(
            Guid.NewGuid(),
            betAmount,
            SlotSymbol.Leaf,
            SlotSymbol.Leaf,
            SlotSymbol.Leaf);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Theory]
    [InlineData(0.501)]
    [InlineData(1.999)]
    [InlineData(10.123)]
    public void Create_ShouldThrow_WhenBetHasMoreThanTwoDecimalPlaces(
        decimal betAmount)
    {
        var action = () => GameRound.Create(
            Guid.NewGuid(),
            betAmount,
            SlotSymbol.Leaf,
            SlotSymbol.Leaf,
            SlotSymbol.Leaf);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Create_ShouldThrow_WhenUserIdIsEmpty()
    {
        var action = () => GameRound.Create(
            Guid.Empty,
            10m,
            SlotSymbol.Leaf,
            SlotSymbol.Leaf,
            SlotSymbol.Leaf);

        Assert.Throws<ArgumentException>(action);
    }
}