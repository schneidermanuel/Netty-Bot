using System.Collections.Generic;
using DiscordBot.Framework.Contract.Modularity.Commands;

namespace DiscordBot.Framework.RestrictionResolver;

internal interface IRestrictionResolver
{
    bool IsResponsible(string commandName, string parameterName);
    Dictionary<string, string> PermittedValues { get; }
}