﻿using System.Collections.Generic;
using System.Linq;

namespace DiscordBot.DataAccess.Contract.MkCalculator;

public class MkGame
{
    public IList<MkResult> Races { get; set; } = new List<MkResult>();

    public MkResult Totals => new()
    {
        Points = Races.Sum(r => r.Points),
        EnemyPoints = Races.Sum(r => r.EnemyPoints)
    };

    public long GameId { get; set; }
    public MkTeam Team { get; set; }
    public MkTeam Enemy { get; set; }
}