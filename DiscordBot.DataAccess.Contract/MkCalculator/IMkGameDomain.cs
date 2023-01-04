using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Contract.MkCalculator;

public interface IMkGameDomain
{
    Task ClearAsync(ulong channelId);
    Task<long> SaveOrUpdateAsync(ulong channelId, ulong guildId, MkResult gameToSave);
    Task SaveHistoryItemAsync(MkHistoryItem history);
    Task<bool> CanRevertAsync(long gameId);
    Task<MkHistoryItem> RevertGameAsync(long gameId);
    Task<IEnumerable<MkHistoryItem>> RetriveHistoryAsync(long gameId);
    Task<IReadOnlyCollection<ulong>> RetriveChannelsToStopAsync(DateTime dueDate);
}