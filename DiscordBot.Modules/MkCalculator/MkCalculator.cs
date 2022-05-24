using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordBot.Modules.MkCalculator;

internal class MkCalculator : IMkCalculator
{
    private static readonly Dictionary<int, int> ScorePointLookup = new()
    {
        { 1, 15 },
        { 2, 12 },
        { 3, 10 },
        { 4, 9 },
        { 5, 8 },
        { 6, 7 },
        { 7, 6 },
        { 8, 5 },
        { 9, 4 },
        { 10, 3 },
        { 11, 2 },
        { 12, 1 },
    };

    private const int TotalScore = 82;

    public MkResult Calculate(IReadOnlyList<int> places)
    {
        var teamPoints = places.Sum(place => ScorePointLookup[place]);
        var enemyPoints = TotalScore - teamPoints;
        var difference = Math.Abs(TotalScore - enemyPoints);
        return new MkResult
        {
            Difference = difference,
            Points = teamPoints,
            EnemyPoints = enemyPoints
        };
    }
}