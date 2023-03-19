namespace DiscordBot.PubSub.Backend.Data.Guild.ReactionRole;

[Serializable]
internal class ReactionRole
{
    public long Id { get; set; }
    public string ChannelName { get; set; }
    public string MessageContent { get; set; }
    public bool IsUrlEmote { get; set; }
    public string Url { get; set; }
    public string UnicodeEmote { get; set; }
    public ulong RoleId { get; set; }
}