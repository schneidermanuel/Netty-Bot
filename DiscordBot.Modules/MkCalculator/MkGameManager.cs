using System.Collections.Generic;
using DiscordBot.DataAccess.Contract.MkCalculator;

namespace DiscordBot.Modules.MkCalculator;

internal class MkManager
{
    private readonly IMkGameBusinessLogic _businessLogic;
    private readonly Dictionary<ulong, MkResult> _runningGames = new();

    public MkManager(IMkGameBusinessLogic businessLogic)
    {
        _businessLogic = businessLogic;
    }
    
    public void RegisterResult(MkResult result, ulong userId)
    {
        if (!_runningGames.ContainsKey(userId))
        {
            _runningGames.Add(userId, result);
            _businessLogic.SaveOrUpdate(userId, result);
            return;
        }

        var game = _runningGames[userId];
        game.Points += result.Points;
        game.EnemyPoints += result.EnemyPoints;
        _businessLogic.SaveOrUpdate(userId, game);
    }
    
    public MkResult GetFinalResult(ulong userId)
    {
        if (!_runningGames.ContainsKey(userId))
        {
            return null;
        }

        var game = _runningGames[userId];
        return game;
    }

    public void EndGame(ulong userId)
    {
        if (_runningGames.ContainsKey(userId))
        {
            _runningGames.Remove(userId);
            _businessLogic.ClearAsync(userId);
        }
    }
}