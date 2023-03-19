using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBot.DataAccess.Contract.AutoMod.Violation;

public class WarnUserAction : IRuleViolationAction
{

    public WarnUserAction(string reason)
    {
        Reason = reason;
    }

    public async Task Execute(ICommandContext context, string reason)
    {
        await context.Channel.SendMessageAsync(reason);
        await context.Message.DeleteAsync();

    }

    public int Priority => 10;
    public string Reason { get; }
}