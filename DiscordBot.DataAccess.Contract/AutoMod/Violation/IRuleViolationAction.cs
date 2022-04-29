using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBot.DataAccess.Contract.AutoMod.Violation;

public interface IRuleViolationAction
{
    Task Execute(ICommandContext context);
    int Priority { get; }
}