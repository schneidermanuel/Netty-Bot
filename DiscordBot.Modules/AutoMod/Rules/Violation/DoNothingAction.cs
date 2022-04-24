using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBot.Modules.AutoMod.Rules.Violation;

internal class DoNothingAction : IRuleViolationAction
{
    public async Task Execute(ICommandContext context)
    {
        await Task.CompletedTask;
    }

    public int Priority => 1;
}