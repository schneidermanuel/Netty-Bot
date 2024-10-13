using System.Threading.Tasks;

namespace DiscordBot.Framework.Contract.Modules.Tournaments;

public interface ITournamentCompletionDomain
{
    Task CompleteTournamentAsync(string code);
}