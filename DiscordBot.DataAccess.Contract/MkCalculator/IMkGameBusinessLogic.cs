using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Contract.MkCalculator;

public interface IMkGameBusinessLogic
{
    Task ClearAllAsync();
    Task ClearAsync(ulong channelId);
    Task SaveOrUpdate(ulong channelId, MkResult gameToSave);
}