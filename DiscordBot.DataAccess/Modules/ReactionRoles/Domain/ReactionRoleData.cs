namespace DiscordBot.DataAccess.Modules.ReactionRoles.Domain;

internal class ReactionRoleData
{
    public ReactionRoleData(long reactionRoleId, string guildId, string channelId, string messageId, string emojiId,
        string roleId, bool isEmoji)
    {
        ReactionRoleId = reactionRoleId;
        GuildId = guildId;
        ChannelId = channelId;
        MessageId = messageId;
        EmojiId = emojiId;
        RoleId = roleId;
        IsEmoji = isEmoji;
    }

    public ReactionRoleData(long reactionRoleId, ulong guildId, ulong channelId, ulong messageId, string emojiId,
        ulong roleId, bool isEmoji)
    {
        ReactionRoleId = reactionRoleId;
        GuildId = guildId.ToString();
        ChannelId = channelId.ToString();
        MessageId = messageId.ToString();
        EmojiId = emojiId;
        RoleId = roleId.ToString();
        IsEmoji = isEmoji;
    }

    public long ReactionRoleId { get; }
    public string GuildId { get; }
    public string ChannelId { get; }
    public string MessageId { get; }
    public string EmojiId { get; }
    public string RoleId { get; }
    public bool IsEmoji { get; }
}