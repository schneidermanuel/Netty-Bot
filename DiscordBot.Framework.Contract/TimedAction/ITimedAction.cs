using System.Threading.Tasks;
using Discord.WebSocket;

namespace DiscordBot.Framework.Contract.TimedAction;

public interface ITimedAction
{
    ExecutionTime GetExecutionTime();
    Task Execute(DiscordSocketClient client);
}