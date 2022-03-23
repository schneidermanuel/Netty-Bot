using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Contract.AutoRole
{
    public interface IAutoRoleBusinessLogic
    {
        Task<bool> CanCreateAutoRoleAsync(ulong guildId, ulong roleId);
        Task SaveSetupAsync(AutoRoleSetup setup);
        Task<IEnumerable<AutoRoleSetup>> RetrieveAllSetupsForGuildAsync(ulong guildId);
        Task DeleteSetupAsync(long autoRoleSetupId);
        Task<IEnumerable<AutoRoleSetup>> RetrieveAllSetupsAsync();
    }
}