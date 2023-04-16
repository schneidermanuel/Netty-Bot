namespace DiscordBot.DataAccess.Contract.MkCalculator;

public class MkHistoryItem
{
    public long Id { get; set; }
    public long GameId { get; set; }
    public int TeamPoints { get; set; }
    public int EnemyPoints { get; set; }
    public string Comment { get; set; }
    public string Map { get; set; }
}