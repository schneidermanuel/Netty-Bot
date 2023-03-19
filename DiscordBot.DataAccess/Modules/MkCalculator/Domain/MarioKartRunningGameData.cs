using System;

namespace DiscordBot.DataAccess.Modules.MkCalculator.Domain;

internal class MarioKartRunningGameData
{
    public long GameId { get; }
    public string ChannelId { get; }
    public int TeamPoints { get; }
    public int EnemyPoints { get; }
    public int Difference { get; }
    public bool IsCompleted { get; }
    public string GameName { get; }
    public string GuildId { get; }


    public MarioKartRunningGameData(long gameId, ulong channelId, ulong guildId, int teamPoints, int enemyPoints, string gameName, bool isCompleted)
    {
        GameId = gameId;
        ChannelId = channelId.ToString();
        GuildId = guildId.ToString();
        TeamPoints = teamPoints;
        EnemyPoints = enemyPoints;
        GameName = gameName;
        IsCompleted = isCompleted;
        Difference = Math.Abs(teamPoints - enemyPoints);
    }

    public MarioKartRunningGameData(long gameId, string channelId, string guildId, int teamPoints, int enemyPoints,
        string gameName, bool isCompleted)
    {
        GameId = gameId;
        ChannelId = channelId;
        GuildId = guildId;
        TeamPoints = teamPoints;
        EnemyPoints = enemyPoints;
        Difference = Math.Abs(teamPoints - enemyPoints);
        GameName = gameName;
        IsCompleted = isCompleted;
    }
}