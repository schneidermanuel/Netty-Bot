using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBot.Modules.AutoMod.Rules.Violation;

internal class DeleteMessageAction : IRuleViolationAction
{
    public async Task Execute(ICommandContext context)
    {
        await context.Message.DeleteAsync();
    }

    public int Priority => 3;
}