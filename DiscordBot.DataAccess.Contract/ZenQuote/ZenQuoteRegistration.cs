namespace DiscordBot.DataAccess.Contract.ZenQuote
{
    public class ZenQuoteRegistration
    {
        public long Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong Channelid { get; set; }
    }
}