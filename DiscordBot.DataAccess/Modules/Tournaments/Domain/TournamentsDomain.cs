using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.Tournaments.Domain;

namespace DiscordBot.DataAccess.Modules.Tournaments.Domain;

internal class TournamentsDomain : ITournamentsDomain
{
    private readonly ITournamentRepository _repository;

    public TournamentsDomain(ITournamentRepository repository)
    {
        _repository = repository;
    }

    public async Task<CanJoinResult> CanJoinTournamentAsync(ulong userId, ulong guildId, string code)
    {
        var tournament = await _repository.RetrieveDataByCodeAsync(code);
        if (tournament == null)
        {
            return CanJoinResult.No("NOT_FOUND");
        }

        if (tournament.Status != "join")
        {
            return CanJoinResult.No("STATUS_NOT_OPEN");
        }

        if (tournament.Users.Contains(userId))
        {
            return CanJoinResult.No("ALREADY_JOINED");
        }

        if (tournament.GuildId != guildId)
        {
            return CanJoinResult.No("NOT_FOUND");
        }

        return CanJoinResult.Yes();
    }

    public async Task<string> RetrieveRoleIdAsync(string code)
    {
        var data = await _repository.RetrieveDataByCodeAsync(code);
        return data.RoleId;
    }

    public async Task<TournamentDetails> RetrieveTournamentDataAsync(string code)
    {
        var data = await _repository.RetrieveDataByCodeAsync(code);
        var role = string.IsNullOrEmpty(data.RoleId)? (ulong?)null : ulong.Parse(data.RoleId);
        var details = new TournamentDetails(data.Users, role, data.GuildId);
        return details;
    }

    public async Task JoinTournamentAsync(ulong userId, string username, string code, string friendcode, bool canHost)
    {
        await _repository.JoinTournamentAsync(userId, username, code, friendcode, canHost);
    }
}