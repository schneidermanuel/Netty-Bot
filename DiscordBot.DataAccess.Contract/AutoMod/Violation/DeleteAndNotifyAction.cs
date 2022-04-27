using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBot.DataAccess.Contract.AutoMod.Violation;

public class DeleteAndNotifyAction : IRuleViolationAction
{
    private string _reason;

    public DeleteAndNotifyAction(string reason)
    {
        _reason = reason;
    }

    public async Task Execute(ICommandContext context)
    {
        await context.Message.DeleteAsync();
        var message = await context.Channel.SendMessageAsync($"{context.User.Mention}: {_reason}");
        await Task.Delay(5000);
        await message.DeleteAsync();
    }

    public int Priority => 4;
}