using System;

namespace DiscordBot.DataAccess.Entities;

public class MkTournamentRegistrationEntity
{
    public virtual long TournamentRegistrationId { get; set; }
    public virtual long TournamentId { get; set; }
    public virtual string DiscordUserId { get; set; }
    public virtual DateTime RegistrationDate { get; set; }
    public virtual string PlayerName { get; set; }
    public virtual string Friendcode { get; set; }
    public virtual bool CanHost  { get; set; }
}