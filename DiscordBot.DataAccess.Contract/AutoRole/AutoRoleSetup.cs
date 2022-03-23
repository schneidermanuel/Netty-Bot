namespace DiscordBot.DataAccess.Contract.AutoRole
{
    public class AutoRoleSetup
    {
        public long AutoRoleSetupId { get; set; }
        public ulong GuildId { get; set; }
        public ulong RoleId { get; set; }
    }
}