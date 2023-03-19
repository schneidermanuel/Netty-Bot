namespace DiscordBot.DataAccess.Modules.AutoRole.Domain;

internal class AutoRoleSetupData
{
    public AutoRoleSetupData(long id, string guildId, string roleId)
    {
        Id = id;
        GuildId = guildId;
        RoleId = roleId;
    }

    public AutoRoleSetupData(long id, ulong guildId, ulong roleId)
    {
        Id = id;
        GuildId = guildId.ToString();
        RoleId = roleId.ToString();
    }

    public long Id { get; }
    public string GuildId { get; }
    public string RoleId { get; }
}