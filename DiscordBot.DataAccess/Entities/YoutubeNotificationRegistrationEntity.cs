namespace DiscordBot.DataAccess.Entities;

public class YoutubeNotificationRegistrationEntity
{
    public virtual long Id { get; set; }
    public virtual string GuildId { get; set; }
    public virtual string ChannelId { get; set; }
    public virtual string Message { get; set; }
    public virtual string YoutubeChannelId { get; set; }
}