using System;

namespace DiscordBot.Framework;

[Serializable]
public class MainConfig
{
    public string DiscordBotToken { get; set; }
    public static bool SkipDaily { get; set; }
    public string TwitchClientId { get; set; }
    public string TwitchClientSecret { get; set; }
    public static bool Debug { get; set; }
    public string Hostname { get; set; }
    public string LavalinkHost { get; set; }
    public ushort LavalinkPort { get; set; }
    public string LavalinkPassword { get; set; }
    public bool LavalinkSsl { get; set; }
    public string YoutubeApiKey { get; set; }
    public int Port { get; set; }
    public string TwitterBearerToken { get; set; }
    public string SpotifyClientId { get; set; }
    public string SpotifyClientSecret { get; set; }

}