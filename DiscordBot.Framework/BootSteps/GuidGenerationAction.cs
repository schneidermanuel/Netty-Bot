using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.DataAccess.Modules.WebAccess.Domain;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Framework.BootSteps;

internal class GuidGenerationAction : ITimedAction
{
    private readonly IWebAccessDomain _domain;

    public GuidGenerationAction(IWebAccessDomain domain)
    {
        _domain = domain;
    }
    public ExecutionTime GetExecutionTime()
    {
        return ExecutionTime.Daily;
    }

    public async Task ExecuteAsync(DiscordSocketClient client)
    {
        await _domain.GenerateNewGuid();
    }
}