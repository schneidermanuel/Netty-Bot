using System.Collections.Generic;
using System.Linq;

namespace DiscordBot.DataAccess.Contract.Tournaments.Domain;

public class TournamentDetails
{
    public IReadOnlyCollection<ulong> UserIds { get; }
    public ulong? RoleId { get; }
    public ulong GuildId { get; }

    public TournamentDetails(IEnumerable<ulong> userIds, ulong? roleId, ulong guildId)
    {
        UserIds = userIds.ToArray();
        RoleId = roleId;
        GuildId = guildId;
    }
}