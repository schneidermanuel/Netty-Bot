namespace DiscordBot.DataAccess.Entities;

public class ZenQuoteRegistrationEntity
{
    public virtual long Id { get; set; }
    public virtual string GuildId { get; set; }
    public virtual string ChannelId { get; set; }
}