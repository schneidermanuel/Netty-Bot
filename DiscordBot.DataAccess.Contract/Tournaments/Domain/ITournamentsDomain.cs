using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Contract.Tournaments.Domain;

public interface ITournamentsDomain
{
    Task<CanJoinResult> CanJoinTournamentAsync(ulong userId, ulong guildId, string code);
    Task JoinTournamentAsync(ulong userId, string username, string code, string friendcode, bool canHost);
    Task<string> RetrieveRoleIdAsync(string code);
    Task<TournamentDetails> RetrieveTournamentDataAsync(string code);
}