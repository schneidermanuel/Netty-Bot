using System;

namespace DiscordBot.DataAccess.Entities;

public class MkTournamentEntity
{
    public virtual long TournamentId { get; set; }
    public virtual string Name { get; set; }
    public virtual string OrganisatorDcId { get; set; }
    public virtual string Status { get; set; }
    public virtual string GuildId { get; set; }
    public virtual DateTime Created { get; set; }
    public virtual string OrganisatorDisplayName { get; set; }
    public virtual string Identifier { get; set; }
    public virtual string RoleId { get; set; }
    
}