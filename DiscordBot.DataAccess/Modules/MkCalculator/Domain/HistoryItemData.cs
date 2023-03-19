namespace DiscordBot.DataAccess.Modules.MkCalculator.Domain;

internal class HistoryItemData
{
    public long Id { get; }
    public long GameId { get; }
    public int Points { get; }
    public int EnemyPoints { get; }
    public string Comment { get; }

    public HistoryItemData(long id, long gameId, int points, int enemyPoints, string comment)
    {
        Id = id;
        GameId = gameId;
        Points = points;
        EnemyPoints = enemyPoints;
        Comment = comment;
    }
}