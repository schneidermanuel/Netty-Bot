using System;

namespace DiscordBot.DataAccess.Modules.MkCalculator.Domain;

internal class MarioKartRunningGameData
{
    public long GameId { get; }
    public string ChannelId { get; }
    public int TeamPoints { get; }
    public int EnemyPoints { get; }
    public int Difference { get; }
    
    
    public MarioKartRunningGameData(long gameId, ulong userId, int teamPoints, int enemyPoints)
    {
        GameId = gameId;
        ChannelId = userId.ToString();
        TeamPoints = teamPoints;
        EnemyPoints = enemyPoints;
        Difference = Math.Abs(teamPoints - enemyPoints);
    }

    public MarioKartRunningGameData(long gameId, string channelId, int teamPoints, int enemyPoints)
    {
        GameId = gameId;
        ChannelId = channelId;
        TeamPoints = teamPoints;
        EnemyPoints = enemyPoints;
        Difference = Math.Abs(teamPoints - enemyPoints);
    }

}