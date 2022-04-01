using System;

namespace DiscordBot.Framework;

[Serializable]
public class MainConfig
{
    public string DiscordBotToken { get; set; }
    public static bool SkipDaily { get; set; }
    public string TwitchClientId { get; set; }
    public string TwitchClientSecret { get; set; }
}