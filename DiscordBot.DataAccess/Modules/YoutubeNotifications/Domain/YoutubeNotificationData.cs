namespace DiscordBot.DataAccess.Modules.YoutubeNotifications.Domain;

internal class YoutubeNotificationData
{
    public YoutubeNotificationData(long id, string guildId, string channelId, string message, string youtubeChannelId)
    {
        Id = id;
        GuildId = guildId;
        ChannelId = channelId;
        Message = message;
        YoutubeChannelId = youtubeChannelId;
    }

    public YoutubeNotificationData(long id, ulong guildId, ulong channelId, string message, string youtubeChannelId)
    {
        Id = id;
        GuildId = guildId.ToString();
        ChannelId = channelId.ToString();
        Message = message;
        YoutubeChannelId = youtubeChannelId;
    }

    public long Id { get; }
    public string GuildId { get; }
    public string ChannelId { get; }
    public string Message { get; }
    public string YoutubeChannelId { get; }
}