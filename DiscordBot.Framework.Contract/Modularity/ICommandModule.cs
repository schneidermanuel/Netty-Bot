using System.Collections.Generic;
using System.Reflection;

namespace DiscordBot.Framework.Contract.Modularity;

public interface ICommandModule
{
    Dictionary<string, MethodInfo> BuildCommandInfos();
    string ModuleUniqueIdentifier { get; }
}