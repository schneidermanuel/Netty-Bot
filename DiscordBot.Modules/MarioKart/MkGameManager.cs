using System.Collections.Generic;
using DiscordBot.DataAccess.Contract.MkCalculator;

namespace DiscordBot.Modules.MarioKart;

internal class MkManager
{
    private readonly IMkGameBusinessLogic _businessLogic;
    private readonly Dictionary<ulong, MkResult> _runningGames = new();

    public MkManager(IMkGameBusinessLogic businessLogic)
    {
        _businessLogic = businessLogic;
    }
    
    public void RegisterResult(MkResult result, ulong channel)
    {
        if (!_runningGames.ContainsKey(channel))
        {
            _runningGames.Add(channel, result);
            _businessLogic.SaveOrUpdate(channel, result);
            return;
        }

        var game = _runningGames[channel];
        game.Points += result.Points;
        game.EnemyPoints += result.EnemyPoints;
        _businessLogic.SaveOrUpdate(channel, game);
    }
    
    public MkResult GetFinalResult(ulong channelId)
    {
        if (!_runningGames.ContainsKey(channelId))
        {
            return null;
        }

        var game = _runningGames[channelId];
        return game;
    }

    public void EndGame(ulong channelId)
    {
        if (_runningGames.ContainsKey(channelId))
        {
            _runningGames.Remove(channelId);
            _businessLogic.ClearAsync(channelId);
        }
    }
}