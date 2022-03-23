namespace DiscordBot.DataAccess.Contract.GeburtstagList
{
    public class BirthdaySubChannel
    {
        public long Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
    }
}