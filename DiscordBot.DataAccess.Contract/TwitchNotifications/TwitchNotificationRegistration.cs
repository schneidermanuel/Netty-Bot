namespace DiscordBot.DataAccess.Contract.TwitchNotifications
{
    public class TwitchNotificationRegistration
    {
        public long RegistrationId { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public string Message { get; set; }
        public string Streamer { get; set; }
    }
}