using System.Collections.Generic;
using DiscordBot.Shared.MarioKart;

namespace DiscordBot.Framework.RestrictionResolver;

internal class MarioKartRestrictionResolver : IRestrictionResolver
{
    public bool IsResponsible(string commandName, string parameterName)
    {
        return commandName == "race" && parameterName == "map";
    }

    public Dictionary<string, string> PermittedValues => MarioKartMapProvider.Maps;
}