using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBot.Modules.AutoMod.Rules.Violation;

public interface IRuleViolationAction
{
    Task Execute(ICommandContext context);
    int Priority { get; }
}