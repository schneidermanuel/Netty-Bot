using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Modules.AutoRole.Repository;

public interface IAutoRoleRepository
{
    Task<bool> CanCreateAutoRoleAsync(ulong guildId, ulong roleId);

    Task SaveSetupAsync
        (AutoRoleSetupData data);

    Task<IEnumerable<AutoRoleSetupData>> RetrieveAllSetupsForGuildAsync(ulong guildId);
    Task DeleteSetupAsync(long autoRoleSetupId);
    Task<IEnumerable<AutoRoleSetupData>> RetrieveAllSetupsAsync();
}