using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBot.Modules.AutoMod.Rules.Violation;

internal class WarnUserAction : IRuleViolationAction
{
    private string _reason;

    public WarnUserAction(string reason)
    {
        _reason = reason;
    }

    public async Task Execute(ICommandContext context)
    {
        await context.Channel.SendMessageAsync(_reason);
        await context.Message.DeleteAsync();
        // TODO: WARN
    }

    public int Priority => 10;
}