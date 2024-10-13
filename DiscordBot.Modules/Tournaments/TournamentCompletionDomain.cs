using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.Tournaments.Domain;
using DiscordBot.Framework.Contract.Modules.Tournaments;

namespace DiscordBot.Modules.Tournaments;

internal class TournamentCompletionDomain : ITournamentCompletionDomain
{
    private readonly DiscordSocketClient _client;
    private readonly ITournamentsDomain _domain;

    public TournamentCompletionDomain(DiscordSocketClient client, ITournamentsDomain domain)
    {
        _client = client;
        _domain = domain;
    }
    public async Task CompleteTournamentAsync(string code)
    {
        var data = await _domain.RetrieveTournamentDataAsync(code);
        if (!data.RoleId.HasValue)
        {
            return;
        }

        var guild = _client.GetGuild(data.GuildId);
        var usersInGuild = (await guild.GetUsersAsync().ToArrayAsync()).SelectMany(x => x.ToArray()).ToArray();
        foreach (var userId in data.UserIds)
        {
            var user = usersInGuild.SingleOrDefault(u => u.Id == userId);
            if (user != null)
            {
                await user.RemoveRoleAsync(data.RoleId.Value);
            }
        }

    }
}