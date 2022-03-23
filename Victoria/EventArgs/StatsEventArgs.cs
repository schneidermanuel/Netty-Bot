using System;
using System.Text.Json.Serialization;
using Victoria.Converters;

namespace Victoria.EventArgs;

/// <summary>
///     Information about Lavalink statistics.
/// </summary>
public sealed class StatsEventArgs
{
    /// <summary>
    ///     Machine's CPU info.
    /// </summary>
    [JsonPropertyName("cpu")]
    public Cpu Cpu { get; private set; }

    /// <summary>
    ///     Audio frames.
    /// </summary>
    [JsonPropertyName("frames")]
    public Frames Frames { get; private set; }

    /// <summary>
    ///     General memory information about Lavalink.
    /// </summary>
    [JsonPropertyName("memory")]
    public Memory Memory { get; private set; }

    /// <summary>
    ///     Connected players.
    /// </summary>
    [JsonPropertyName("players")]
    public int Players { get; private set; }

    /// <summary>
    ///     Players that are currently playing.
    /// </summary>
    [JsonPropertyName("playingPlayers")]
    public int PlayingPlayers { get; private set; }

    /// <summary>
    ///     Lavalink uptime.
    /// </summary>
    [JsonPropertyName("uptime"), JsonConverter(typeof(LongToTimeSpanConverter))]
    public TimeSpan Uptime { get; private set; }
}

/// <summary>
///     General memory information about Lavalink.
/// </summary>
public struct Memory
{
    /// <summary>
    ///     Memory used by Lavalink.
    /// </summary>
    [JsonPropertyName("used")]
    public ulong Used { get; private set; }

    /// <summary>
    ///     Some JAVA stuff.
    /// </summary>
    [JsonPropertyName("free")]
    public ulong Free { get; private set; }

    /// <summary>
    ///     Memory allocated by Lavalink.
    /// </summary>
    [JsonPropertyName("allocated")]
    public ulong Allocated { get; private set; }

    /// <summary>
    ///     Reserved memory?
    /// </summary>
    [JsonPropertyName("reservable")]
    public ulong Reservable { get; private set; }
}

/// <summary>
///     Audio frames.
/// </summary>
public struct Frames
{
    /// <summary>
    ///     Audio frames sent.
    /// </summary>
    [JsonPropertyName("sent")]
    public int Sent { get; private set; }

    /// <summary>
    ///     Frames that were null.
    /// </summary>
    [JsonPropertyName("nulled")]
    public int Nulled { get; private set; }

    /// <summary>
    ///     Frame deficit.
    /// </summary>
    [JsonPropertyName("deficit")]
    public int Deficit { get; private set; }
}

/// <summary>
///     Machine's CPU info.
/// </summary>
public struct Cpu
{
    /// <summary>
    ///     CPU Cores.
    /// </summary>
    [JsonPropertyName("cores")]
    public int Cores { get; private set; }

    /// <summary>
    ///     General load on CPU.
    /// </summary>
    [JsonPropertyName("systemLoad")]
    public double SystemLoad { get; private set; }

    /// <summary>
    ///     Lavalink process load on CPU.
    /// </summary>
    [JsonPropertyName("lavalinkLoad")]
    public double LavalinkLoad { get; private set; }
}