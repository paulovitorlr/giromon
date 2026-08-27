using Giromon.Domain.Enums;
using Giromon.Domain.Services;

namespace Giromon.Domain.Entities;

public class GameRound
{
    public const decimal MinimumBetAmount = 0.50m;

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public decimal BetAmount { get; private set; }
    public SlotSymbol FirstSymbol { get; private set; }
    public SlotSymbol SecondSymbol { get; private set; }
    public SlotSymbol ThirdSymbol { get; private set; }
    public decimal PrizeAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private GameRound()
    {
    }

    private GameRound(
        Guid id,
        Guid userId,
        decimal betAmount,
        SlotSymbol firstSymbol,
        SlotSymbol secondSymbol,
        SlotSymbol thirdSymbol,
        decimal prizeAmount,
        DateTime createdAt)
    {
        Id = id;
        UserId = userId;
        BetAmount = betAmount;
        FirstSymbol = firstSymbol;
        SecondSymbol = secondSymbol;
        ThirdSymbol = thirdSymbol;
        PrizeAmount = prizeAmount;
        CreatedAt = createdAt;
    }

    public static GameRound Create(
        Guid userId,
        decimal betAmount,
        SlotSymbol firstSymbol,
        SlotSymbol secondSymbol,
        SlotSymbol thirdSymbol)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "O identificador do usuário é obrigatório.",
                nameof(userId));
        }

        if (betAmount < MinimumBetAmount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(betAmount),
                $"A aposta mínima é de {MinimumBetAmount} crédito.");
        }

        if (decimal.Round(betAmount, 2) != betAmount)
        {
            throw new ArgumentException(
                "A aposta deve possuir no máximo duas casas decimais.",
                nameof(betAmount));
        }

        ValidateSymbol(firstSymbol, nameof(firstSymbol));
        ValidateSymbol(secondSymbol, nameof(secondSymbol));
        ValidateSymbol(thirdSymbol, nameof(thirdSymbol));

        var prizeAmount = PrizeCalculator.Calculate(
            betAmount,
            firstSymbol,
            secondSymbol,
            thirdSymbol);

        return new GameRound(
            Guid.NewGuid(),
            userId,
            betAmount,
            firstSymbol,
            secondSymbol,
            thirdSymbol,
            prizeAmount,
            DateTime.UtcNow);
    }

    private static void ValidateSymbol(
        SlotSymbol symbol,
        string parameterName)
    {
        if (!Enum.IsDefined(symbol))
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                "Símbolo inválido.");
        }
    }
}