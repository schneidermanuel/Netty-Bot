using System;

namespace DiscordBot.Framework;

[Serializable]
public class MainConfig
{
    public string DiscordBotToken { get; set; }
    public string DatabaseConnectionString { get; set; }
    public static bool SkipDaily { get; set; }
}