using System.Collections.Generic;

namespace DiscordBot.Framework.Contract.Modularity.Commands.RestrictionResolver;

public interface IRestrictionResolver
{
    bool IsResponsible(ParameterRestrictionType type);
    Dictionary<string, string> PermittedValues { get; }
}