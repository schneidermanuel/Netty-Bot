using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Contract.MkCalculator;

public interface IMkGameDomain
{
    Task ClearAsync(ulong channelId);
    Task<long> StartGameAsync(ulong channelId, ulong guildId, MkGame gameToSave);
    Task<long> SaveRaceAsync(MkResult result, long gameId);
    Task<bool> CanRevertAsync(long gameId);
    Task RevertGameAsync(long gameId);
    Task<MkGame> RetrieveGameAsync(long gameId);
    Task<IReadOnlyCollection<ulong>> RetriveChannelsToStopAsync(DateTime dueDate);
}