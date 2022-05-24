using System.Collections.Generic;
using Org.BouncyCastle.Math.EC;

namespace DiscordBot.Modules.MkCalculator;

internal class MkManager
{
    private readonly Dictionary<ulong, MkResult> _runningGames = new();

    public void RegisterResult(MkResult result, ulong guildId)
    {
        if (!_runningGames.ContainsKey(guildId))
        {
            _runningGames.Add(guildId, result);
            return;
        }

        var game = _runningGames[guildId];
        game.Points += result.Points;
        game.EnemyPoints += result.EnemyPoints;
    }

    public void Reset(ulong guildId)
    {
        if (_runningGames.ContainsKey(guildId))
        {
            _runningGames.Remove(guildId);
        }
    }

    public MkResult GetFinalResult(ulong guildId)
    {
        if (!_runningGames.ContainsKey(guildId))
        {
            return null;
        }

        var game = _runningGames[guildId];
        return game;
    }

    public void EndGame(ulong guildId)
    {
        if (_runningGames.ContainsKey(guildId))
        {
            _runningGames.Remove(guildId);
        }
    }
}