using System.Collections.Generic;
using DiscordBot.DataAccess.Contract.MkCalculator;

namespace DiscordBot.Modules.MarioKart;

internal interface IMkCalculator
{
    MkResult Calculate(IReadOnlyList<int> places);
}