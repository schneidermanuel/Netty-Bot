using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Entities;
using DiscordBot.DataAccess.Modules.Tournaments.Domain;
using DiscordBot.DataAccess.NHibernate;
using NHibernate.Linq;

namespace DiscordBot.DataAccess.Modules.Tournaments.Repository;

internal class TournamentRepository : ITournamentRepository
{
    private readonly ISessionProvider _sessionProvider;

    public TournamentRepository(ISessionProvider sessionProvider)
    {
        _sessionProvider = sessionProvider;
    }

    public async Task<TournamentData> RetrieveDataByCodeAsync(string code)
    {
        using (var session = _sessionProvider.OpenSession())
        {
            var tournamentQuery = session.Query<MkTournamentEntity>()
                .Where(entity => entity.Identifier == code);
            var tournament = await tournamentQuery.SingleOrDefaultAsync();
            if (tournament == null)
            {
                return null;
            }

            var registrations = await session.Query<MkTournamentRegistrationEntity>()
                .Where(entity => entity.TournamentId == tournament.TournamentId)
                .ToListAsync();
            var players = registrations
                .Select(entity => ulong.Parse(entity.DiscordUserId))
                .ToArray();
            var data = new TournamentData(tournament.Status, players);
            return data;
        }
    }

    public async Task JoinTournamentAsync(ulong userId, string username, string code, string friendcode, bool canHost)
    {
        using (var session = _sessionProvider.OpenSession())
        {
            var tournamentQuery = session.Query<MkTournamentEntity>()
                .Where(entity => entity.Identifier == code);
            var tournament = await tournamentQuery.SingleAsync();
            var registration = new MkTournamentRegistrationEntity
            {
                CanHost = canHost,
                DiscordUserId = userId.ToString(),
                Friendcode = friendcode,
                PlayerName = username,
                TournamentId = tournament.TournamentId,
            };
            await session.SaveAsync(registration);
        }
    }
}