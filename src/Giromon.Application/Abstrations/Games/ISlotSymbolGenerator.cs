using Giromon.Domain.Enums;

namespace Giromon.Application.Abstractions.Games;

public interface ISlotSymbolGenerator
{
    SlotSymbol Generate();
}