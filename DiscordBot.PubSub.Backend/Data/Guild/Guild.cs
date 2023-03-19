namespace DiscordBot.PubSub.Backend.Data.Guild;

[Serializable]
internal class Guild
{
    public string Name { get; set; }
    public string IconUrl { get; set; }
    public IEnumerable<Module> Modules { get; set; }
}