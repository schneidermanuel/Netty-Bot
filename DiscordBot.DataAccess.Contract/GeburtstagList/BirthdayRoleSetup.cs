namespace DiscordBot.DataAccess.Contract.GeburtstagList
{
    public class BirthdayRoleSetup
    {
        public long SetupId { get; set; }
        public ulong GuildId { get; set; }
        public ulong RoleId { get; set; }
    }
}