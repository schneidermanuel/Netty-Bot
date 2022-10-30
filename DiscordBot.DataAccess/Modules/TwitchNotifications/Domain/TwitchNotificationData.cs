namespace DiscordBot.DataAccess.Modules.TwitchNotifications.Domain;

internal class TwitchNotificationData
{
    public TwitchNotificationData(long id, string guildId, string channelId, string message, string streamer)
    {
        Id = id;
        GuildId = guildId;
        ChannelId = channelId;
        Message = message;
        Streamer = streamer;
    }

    public TwitchNotificationData(long id, ulong guildId, ulong channelId, string message, string streamer)
    {
        Id = id;
        GuildId = guildId.ToString();
        ChannelId = channelId.ToString();
        Message = message;
        Streamer = streamer;
    }

    public long Id { get; }
    public string GuildId { get; }
    public string ChannelId { get; }
    public string Message { get; }
    public string Streamer { get; }
}