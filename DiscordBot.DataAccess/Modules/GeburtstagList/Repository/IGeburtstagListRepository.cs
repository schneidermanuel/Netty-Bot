using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Modules.GeburtstagList.BusinessLogic;

namespace DiscordBot.DataAccess.Modules.GeburtstagList.Repository;

public interface IGeburtstagListRepository
{
    Task<bool> HasGuildSetupGeburtstagChannelAsync(ulong guildId);
    Task<long> SaveBirthdayChannelAsync(BirthdayChannelData data);
    Task<IEnumerable<BirthdayChannelData>> GetAllGeburtstagsChannelAsync();
    Task<IEnumerable<BirthdayData>> GetAllGeburtstageAsync();
    Task DeleteBirthdayChannelAsync(long channelId);
    Task<bool> HasUserRegisteredBirthday(ulong userId);
    Task SaveBirthdayAsync(BirthdayData data);
    Task<IEnumerable<BirthdaySubChannelData>> GetAllSubbedChannelAsync();
    Task SaveBirthdaySubAsync(BirthdaySubChannelData data);
    Task<bool> IsChannelSubbedAsync(ulong guildId, ulong channelId);
    Task DeleteSubbedChannelAsync(string guildId, string channeld);
    Task<bool> HasGuildSetupBirthdayRoleAsync(ulong guildId);
    Task UpdateExistingBirthdayRoleSetupAsync(ulong guildId, ulong roleId);
    Task CreateNewBirthdayRoleSetupAsync(ulong guildId, ulong roleId);
    Task<ulong> RetrieveBirthdayRoleIdForGuildAsync(ulong guildId);
    Task InsertBirthdayRoleAssotiation(ulong guildId, ulong userId);
    Task<IEnumerable<BirthdayRoleAssotiationData>> RetrieveAllBirthdayRoleAssotiations();
    Task<IEnumerable<BirthdayRoleSetupData>> RetrieveAllBirthdaySetupsAsync();
    Task DeleteAssociationAsync(long id);
}