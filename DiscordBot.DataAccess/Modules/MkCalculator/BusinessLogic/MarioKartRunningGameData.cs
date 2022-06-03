using System;

namespace DiscordBot.DataAccess.Modules.MkCalculator.BusinessLogic;

internal class MarioKartRunningGameData
{
    public long GameId { get; }
    public string UserId { get; }
    public int TeamPoints { get; }
    public int EnemyPoints { get; }
    public int Difference { get; }
    
    
    public MarioKartRunningGameData(long gameId, ulong userId, int teamPoints, int enemyPoints)
    {
        GameId = gameId;
        UserId = userId.ToString();
        TeamPoints = teamPoints;
        EnemyPoints = enemyPoints;
        Difference = Math.Abs(teamPoints - enemyPoints);
    }

    public MarioKartRunningGameData(long gameId, string userId, int teamPoints, int enemyPoints)
    {
        GameId = gameId;
        UserId = userId;
        TeamPoints = teamPoints;
        EnemyPoints = enemyPoints;
        Difference = Math.Abs(teamPoints - enemyPoints);
    }

}