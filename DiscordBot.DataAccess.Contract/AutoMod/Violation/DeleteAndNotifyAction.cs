using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBot.DataAccess.Contract.AutoMod.Violation;

public class DeleteAndNotifyAction : IRuleViolationAction
{

    public DeleteAndNotifyAction(string reason)
    {
        Reason = reason;
    }
    public async Task Execute(ICommandContext context, string reason)
    {
        await context.Message.DeleteAsync();
        var message = await context.Channel.SendMessageAsync($"{context.User.Mention}: {reason}");
        await Task.Delay(5000);
        await message.DeleteAsync();
    }

    public int Priority => 4;
    public string Reason { get; }
}