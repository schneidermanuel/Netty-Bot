using System;

namespace DiscordBot.DataAccess.Contract.MkCalculator;

public class MkResult
{
    public int Points { get; set; }
    public int Difference => Math.Abs(Points - EnemyPoints);
    public int EnemyPoints { get; set; }
    public int Track { get; set; }
    public string Comment { get; set; }
    public string Map { get; set; }
}