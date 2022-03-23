using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBot.Framework.Contract.Modularity;

public interface IGuildModule
{
    public Task<bool> CanExecuteAsync(ulong id, SocketCommandContext socketCommandContext);
    public Task ExecuteAsync(ICommandContext context);
    public string ModuleUniqueIdentifier { get; }
}