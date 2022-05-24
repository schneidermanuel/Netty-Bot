using System;

namespace DiscordBot.Modules.MkCalculator;

internal class MkResult
{
    public int Points { get; set; }
    public int Difference => Math.Abs(Points - EnemyPoints);
    public int EnemyPoints { get; set; }
}