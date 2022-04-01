namespace DiscordBot.DataAccess.Modules.TwitchNotifications.BusinessLogic;

internal class TwitchNotificationData
{
    public long Id { get; set; }
    public ulong GuildId { get; set; }
    public ulong ChannelId { get; set; }
    public string Streamer { get; set; }
    public string Message { get; set; }
}