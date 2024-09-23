using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Modules.Tournaments.Domain;

internal interface ITournamentRepository
{
    Task<TournamentData> RetrieveDataByCodeAsync(string code);
    Task JoinTournamentAsync(ulong userId, string username, string code, string friendcode, bool canHost);
}