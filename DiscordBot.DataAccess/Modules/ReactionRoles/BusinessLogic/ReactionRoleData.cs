namespace DiscordBot.DataAccess.Modules.ReactionRoles.BusinessLogic;

public class ReactionRoleData
{
    public long ReactionRoleId { get; set; }
    public ulong GuildId { get; set; }
    public ulong ChannelId { get; set; }
    public ulong MessageId { get; set; }
    public string EmojiId { get; set; }
    public ulong RoleId { get; set; }
    public bool IsEmoji { get; set; }
}