namespace DiscordBot.DataAccess.Entities;

public class AutoroleSetupEntity
{
    public virtual long Id { get; set; }
    public virtual string GuildId { get; set; }
    public virtual string RoleId { get; set; }
}