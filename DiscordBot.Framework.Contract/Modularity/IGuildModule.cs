using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBot.Framework.Contract.Modularity;

public interface IGuildModule
{
    Task<bool> CanExecuteAsync(ulong id, SocketCommandContext socketCommandContext);
    Task ExecuteAsync(ICommandContext context);
    string ModuleUniqueIdentifier { get; }
    Task InitializeAsync(SocketCommandContext context);
}