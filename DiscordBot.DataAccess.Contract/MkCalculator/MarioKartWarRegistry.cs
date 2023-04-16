namespace DiscordBot.DataAccess.Contract.MkCalculator;

public class MarioKartWarRegistry
{
    public string TeamName { get; }
    public string TeamImage { get; }
    public string EnemyName { get; }
    public string EnemyImage { get; }

    public MarioKartWarRegistry(
        string teamName,
        string teamImage,
        string enemyName,
        string enemyImage)
    {
        TeamName = teamName;
        TeamImage = teamImage;
        EnemyName = enemyName;
        EnemyImage = enemyImage;
    }
}