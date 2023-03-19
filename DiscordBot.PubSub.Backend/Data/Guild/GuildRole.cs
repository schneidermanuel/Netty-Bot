namespace DiscordBot.PubSub.Backend.Data.Guild;

[Serializable]
internal class GuildRole
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool IsAdmin { get; set; }
}