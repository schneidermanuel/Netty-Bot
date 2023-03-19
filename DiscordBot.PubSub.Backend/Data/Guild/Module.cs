namespace DiscordBot.PubSub.Backend.Data.Guild;

[Serializable]
internal class Module
{
    public string UniqueKey { get; set; }
    public bool Enabled { get; set; }
}