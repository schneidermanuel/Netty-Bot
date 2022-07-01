using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Contract.MkCalculator;

public interface IMkGameBusinessLogic
{
    Task ClearAllAsync();
    Task ClearAsync(ulong channelId);
    Task<long> SaveOrUpdateAsync(ulong channelId, MkResult gameToSave);
    Task SaveHistoryItemAsync(MkHistoryItem history);
    Task<bool> CanRevertAsync(long gameId);
    Task<MkHistoryItem> RevertGameAsync(long gameId);
}