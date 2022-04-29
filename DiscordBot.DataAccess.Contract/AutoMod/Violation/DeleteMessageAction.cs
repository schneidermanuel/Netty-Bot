using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBot.DataAccess.Contract.AutoMod.Violation;

public class DeleteMessageAction : IRuleViolationAction
{
    public async Task Execute(ICommandContext context)
    {
        await context.Message.DeleteAsync();
    }

    public int Priority => 3;
}