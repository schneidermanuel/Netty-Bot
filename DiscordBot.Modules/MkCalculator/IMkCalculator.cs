using System.Collections.Generic;

namespace DiscordBot.Modules.MkCalculator;

internal interface IMkCalculator
{
    MkResult Calculate(IReadOnlyList<int> places);
}