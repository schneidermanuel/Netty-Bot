namespace DiscordBot.PubSub.Backend.Data.Guild.AutoModRule;

[Serializable]
public class AutoModConfig
{
    public string RuleKey { get; set; }
    public AutoModConfigConfiguration[] Configs { get; set; }
}