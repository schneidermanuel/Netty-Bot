using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.ServerCounter;

internal class ServerCoutnerAction : ITimedAction
{
    public ExecutionTime GetExecutionTime()
    {
        return ExecutionTime.Hourly;
    }

    public async Task Execute(DiscordSocketClient client)
    {
        var guildCount = client.Guilds.Count;
        var message = $"We now support english! Change your language with !language. Hosted with love by Brainy for {guildCount} Servers";
        await client.SetActivityAsync(new Game(message));
    }
}