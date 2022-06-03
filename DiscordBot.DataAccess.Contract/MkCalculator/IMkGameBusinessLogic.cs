using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Contract.MkCalculator;

public interface IMkGameBusinessLogic
{
    Task ClearAllAsync();
    Task ClearAsync(ulong userId);
    Task SaveOrUpdate(ulong userId, MkResult gameToSave);
}