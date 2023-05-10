using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.MkCalculator;

namespace DiscordBot.Modules.MarioKart;

internal class MkGameManager
{
    private readonly IMkGameDomain _domain;
    private readonly Dictionary<ulong, MkGame> _runningGames = new();
    private readonly Dictionary<ulong, MkResult> _resultsInProcess = new();

    public MkGameManager(IMkGameDomain domain)
    {
        _domain = domain;
    }

    public bool HasChannelRunningGame(ulong channelId)
    {
        return _runningGames.ContainsKey(channelId);
    }

    public async Task<long> StartGameAsync(ulong channelId, ulong guildId, MkGame game)
    {
        var preparedRace = _resultsInProcess[channelId];
        game.Races.Add(preparedRace);
        var gameId = await _domain.StartGameAsync(channelId, guildId, game);
        _runningGames.Add(channelId, game);
        game.GameId = gameId;
        var id = await RegisterResultAsync(preparedRace, channelId);
        Debug.Assert(id != null, nameof(id) + " != null");
        return id.Value;
    }

    public async Task<long?> RegisterResultAsync(MkResult result, ulong channelId)
    {
        if (!_runningGames.ContainsKey(channelId))
        {
            _resultsInProcess.Add(channelId, result);
            return null;
        }

        var game = _runningGames[channelId];
        result.Track = game.Races.Count + 1;

        return await _domain.SaveRaceAsync(result, game.GameId);
    }

    public MkGame RetrieveGame(ulong channelId)
    {
        if (!_runningGames.ContainsKey(channelId))
        {
            return null;
        }

        var game = _runningGames[channelId];
        return game;
    }

    public async Task<bool> CanRevertAsync(ulong channelId)
    {
        if (!_runningGames.ContainsKey(channelId))
        {
            return false;
        }

        var gameId = _runningGames[channelId].GameId;
        return await _domain.CanRevertAsync(gameId);
    }

    public async Task RevertGameAsync(ulong channelId)
    {
        var game = _runningGames[channelId];
        game.Races.Remove(game.Races.OrderBy(r => r.Track).Last());
        await _domain.RevertGameAsync(game.GameId);
    }

    public async Task EndGameAsync(ulong channelId)
    {
        if (_runningGames.ContainsKey(channelId))
        {
            _runningGames.Remove(channelId);
            await _domain.ClearAsync(channelId);
        }
    }

    public async Task AutoCompleteGamesAsync(DateTime dueDate)
    {
        var gamesToStop = await _domain.RetriveChannelsToStopAsync(dueDate);
        var stopGameTasks = gamesToStop.Select(EndGameAsync);
        await Task.WhenAll(stopGameTasks);
    }
}