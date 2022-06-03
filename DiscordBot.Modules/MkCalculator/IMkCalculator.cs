using System.Collections.Generic;
using DiscordBot.DataAccess.Contract.MkCalculator;

namespace DiscordBot.Modules.MkCalculator;

internal interface IMkCalculator
{
    MkResult Calculate(IReadOnlyList<int> places);
}