namespace DiscordBot.DataAccess.Contract.GeburtstagList
{
    public class BirthdayRoleAssotiation
    {
        public long AssotiationId { get; set; }
        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }
    }
}