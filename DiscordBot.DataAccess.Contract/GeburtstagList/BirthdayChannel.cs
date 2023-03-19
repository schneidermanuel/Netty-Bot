namespace DiscordBot.DataAccess.Contract.GeburtstagList
{
    public class BirthdayChannel
    {
        public long Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong JanMessageId { get; set; }
        public ulong FebMessageId { get; set; }
        public ulong MarMessageId { get; set; }
        public ulong AprMessageId { get; set; }
        public ulong MaiMessageId { get; set; }
        public ulong JunMessageId { get; set; }
        public ulong JulMessageId { get; set; }
        public ulong AugMessageId { get; set; }
        public ulong SepMessageId { get; set; }
        public ulong OctMessageId { get; set; }
        public ulong NovMessageId { get; set; }
        public ulong DezMessageId { get; set; }
    }
}