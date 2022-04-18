namespace DiscordBot.DataAccess.Modules.GeburtstagList.BusinessLogic;

internal class BirthdayRoleSetupData
{
    public BirthdayRoleSetupData(long setupId, string guildId, string roleId)
    {
        SetupId = setupId;
        GuildId = guildId;
        RoleId = roleId;
    }

    public BirthdayRoleSetupData(long setupId, ulong guildId, ulong roleId)
    {
        SetupId = setupId;
        GuildId = guildId.ToString();
        RoleId = roleId.ToString();
    }


    public long SetupId { get; }
    public string GuildId { get; }
    public string RoleId { get; }
}