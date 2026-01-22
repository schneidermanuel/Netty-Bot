using System;

namespace DiscordBot.Framework.Contract;

public static class BotClientConstants
{
    public static string BotToken => Environment.GetEnvironmentVariable("BOT_TOKEN");
    public static string TwitchClientId => Environment.GetEnvironmentVariable("TWITCH_CLIENT_ID");
    public static string TwitchClientSecret => Environment.GetEnvironmentVariable("TWITCH_CLIENT_SECRET");
    public static string Hostname => Environment.GetEnvironmentVariable("HOSTNAME");
    public static string LavalinkHost => Environment.GetEnvironmentVariable("LAVALINK_HOST");
    public static ushort LavalinkPort => ushort.Parse(Environment.GetEnvironmentVariable("LAVALINK_PORT") ?? "2333");
    public static string LavalinkPassword => Environment.GetEnvironmentVariable("LAVALINK_PASSWORD");
    public static bool LavalinkSsl => bool.Parse(Environment.GetEnvironmentVariable("LAVALINK_SSL") ?? "false");
    public static string YoutubeApiKey => Environment.GetEnvironmentVariable("YOUTUBE_API_KEY");
    public static int Port => int.Parse(Environment.GetEnvironmentVariable("PORT") ?? "37878");
    public static string SpotifyClientId => Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
    public static string SpotifyClientSecret => Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET");
    public static string PubSubHubHubSecret => Environment.GetEnvironmentVariable("PUBHUB_SECRET");
}