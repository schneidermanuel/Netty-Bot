using System.Threading.Tasks;
using Discord.WebSocket;

namespace DiscordBot.Framework.Contract.Modularity;

public interface IModalListener
{
    string ButtonEventPrefix { get; }
    Task SubmittedAsync(ulong userId, SocketModal modal);
}