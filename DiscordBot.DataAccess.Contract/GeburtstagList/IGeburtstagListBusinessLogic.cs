using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Contract.GeburtstagList
{
    public interface IGeburtstagListBusinessLogic
    {
        Task<bool> HasGuildSetupGeburtstagChannelAsync(ulong guildId);
        Task<long> SaveBirthdayChannelAsync(BirthdayChannel birthdayChannel);
        Task<IEnumerable<BirthdayChannel>> GetAllGeburtstagsChannelAsync();
        Task<List<Birthday>> GetAllGeburtstageAsync();
        Task DeleteBirthdayChannelAsync(long channelId);
        Task<bool> HasUserRegisteredBirthday(ulong userId);
        Task SaveBirthdayAsync(Birthday registration);
        Task<IEnumerable<BirthdaySubChannel>> GetAllSubbedChannelAsync();
        Task SaveBirthdaySubAsync(BirthdaySubChannel sub);
        Task<bool> IsChannelSubbedAsync(ulong guildId, ulong channelId);
        Task DeleteSubbedChannelAsync(ulong guildId, ulong channelId);
        Task CreateOrUpdateBirthdayRoleAsync(ulong guildId, ulong roleId);
        Task<bool> HasGuildSetupBirthdayRoleAsync(ulong guildId);
        Task<ulong> RetrieveBirthdayRoleIdForGuildAsync(ulong guildId);
        Task InsertBirthdayRoleAssotiation(ulong guildId, ulong userId);
        Task<IEnumerable<BirthdayRoleAssotiation>> RetrieveAllBirthdayRoleAssotiations();
        Task<IEnumerable<BirthdayRoleSetup>> RetrieveAllBirthdayRoleSetupsAsync();
        Task DeleteAssociationAsync(BirthdayRoleAssotiation birthdayRoleAssociation);
    }
}