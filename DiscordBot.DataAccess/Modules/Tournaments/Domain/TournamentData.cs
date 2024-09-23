using System.Collections.Generic;
using System.Linq;

namespace DiscordBot.DataAccess.Modules.Tournaments.Domain;

internal class TournamentData
{
    public string Status { get; }
    public IReadOnlyCollection<ulong> Users { get; }

    public TournamentData(string status, IEnumerable<ulong> users)
    {
        Status = status;
        Users = users.ToArray();
    }
}