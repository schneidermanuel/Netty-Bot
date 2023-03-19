using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace DiscordBot.Framework.Contract.Modularity;

public interface ICommandModule
{
    Dictionary<string, MethodInfo> BuildCommandInfos();
    string ModuleUniqueIdentifier { get; }
    Task InitializeAsync(SocketSlashCommand context);
}