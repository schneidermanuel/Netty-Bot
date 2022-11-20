namespace DiscordBot.PubSub.Backend.Data.Guild;

[Serializable]
internal class Channel
{
    public ulong ChannelId { get; set; }
    public string ChannelName { get; set; }
}