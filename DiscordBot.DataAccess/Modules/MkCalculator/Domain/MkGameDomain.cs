using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.MkCalculator;

namespace DiscordBot.DataAccess.Modules.MkCalculator.Domain;

internal class MkGameDomain : IMkGameDomain
{
    private readonly IMkGameRepository _repository;

    public MkGameDomain(IMkGameRepository repository)
    {
        _repository = repository;
    }

    public async Task ClearAsync(ulong channelId)
    {
        await _repository.ClearAsync(channelId.ToString());
    }

    public async Task<long> StartGameAsync(ulong channelId, ulong guildId, MkGame gameToSave)
    {
        var now = DateTime.Now;
        var gameName = $"{now:dd.MM.yyyy} | {gameToSave.Team.Name} vs {gameToSave.Enemy.Name}";
        return await _repository.SaveGameAsync(gameToSave, gameName, guildId.ToString(), channelId.ToString());
    }

    public async Task<long> SaveRaceAsync(MkResult result, long gameId)
    {
        var raceId = await _repository.SaveRaceAsync(result, DateTime.Now, gameId);
        var game = await _repository.RetrieveGameAsync(gameId);
        await _repository.UpdateTotalsAsync(game);
        return raceId;
    }


    public async Task<bool> CanRevertAsync(long gameId)
    {
        return await _repository.CanRevertAsync(gameId);
    }

    public async Task<MkGame> RetrieveGameAsync(long gameId)
    {
        return await _repository.RetrieveGameAsync(gameId);
    }

    public async Task RevertGameAsync(long gameId)
    {
        await _repository.RevertGameAsync(gameId);
    }

    public async Task<IReadOnlyCollection<ulong>> RetriveChannelsToStopAsync(DateTime dueDate)
    {
        return await _repository.RetrieveChannelsToStopAsync(dueDate);
    }
}