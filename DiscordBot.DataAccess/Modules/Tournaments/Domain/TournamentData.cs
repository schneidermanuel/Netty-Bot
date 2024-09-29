using System.Collections.Generic;
using System.Linq;

namespace DiscordBot.DataAccess.Modules.Tournaments.Domain;

internal class TournamentData
{
    public string Status { get; }
    public ulong GuildId { get; }
    public IReadOnlyCollection<ulong> Users { get; }

    public TournamentData(string status, ulong guildId, IEnumerable<ulong> users)
    {
        Status = status;
        GuildId = guildId;
        Users = users.ToArray();
    }
}