namespace DiscordBot.DataAccess.Entities;

public class BirthdayRoleSetupEntity
{
    public virtual long SetupId { get; set; }
    public virtual string GuildId { get; set; }
    public virtual string RoleId { get; set; }
}