using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBot.DataAccess.Contract.AutoMod.Violation;

public interface IRuleViolationAction
{
    Task Execute(ICommandContext context, string reason);
    int Priority { get; }
    string Reason { get; }
    
}