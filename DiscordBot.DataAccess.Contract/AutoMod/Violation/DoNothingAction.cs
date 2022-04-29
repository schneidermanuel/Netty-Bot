using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBot.DataAccess.Contract.AutoMod.Violation;

public class DoNothingAction : IRuleViolationAction
{
    public async Task Execute(ICommandContext context)
    {
        await Task.CompletedTask;
    }

    public int Priority => 1;
}