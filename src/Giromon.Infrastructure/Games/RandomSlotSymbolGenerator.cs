using Giromon.Application.Abstractions.Games;
using Giromon.Domain.Enums;

namespace Giromon.Infrastructure.Games;

public class RandomSlotSymbolGenerator : ISlotSymbolGenerator
{
    private static readonly SlotSymbol[] Symbols =
        Enum.GetValues<SlotSymbol>();

    public SlotSymbol Generate()
    {
        var index = Random.Shared.Next(Symbols.Length);

        return Symbols[index];
    }
}