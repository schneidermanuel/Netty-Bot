namespace DiscordBot.DataAccess.Contract.YoutubeNotification;

public class YoutubeNotificationRegistration
{
    public long RegistrationId { get; set; }
    public ulong GuildId { get; set; }
    public ulong ChannelId { get; set; }
    public string Message { get; set; }
    public string YoutubeChannelId { get; set; }
}