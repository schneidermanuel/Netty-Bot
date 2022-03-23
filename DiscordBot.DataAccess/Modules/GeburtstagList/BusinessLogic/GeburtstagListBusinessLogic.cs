using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.GeburtstagList;
using DiscordBot.DataAccess.Modules.GeburtstagList.Repository;

namespace DiscordBot.DataAccess.Modules.GeburtstagList.BusinessLogic;

public class GeburtstagListBusinessLogic : IGeburtstagListBusinessLogic
{
    private readonly IGeburtstagListRepository _repository;

    public GeburtstagListBusinessLogic(IGeburtstagListRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> HasGuildSetupGeburtstagChannelAsync(ulong guildId)
    {
        return await _repository.HasGuildSetupGeburtstagChannelAsync(guildId);
    }

    public async Task<long> SaveBirthdayChannelAsync(BirthdayChannel birthdayChannel)
    {
        var data = MapChannelToData(birthdayChannel);
        return await _repository.SaveBirthdayChannelAsync(data);
    }

    private static BirthdayChannelData MapChannelToData(BirthdayChannel birthdayChannel)
    {
        var data = new BirthdayChannelData
        {
            Id = birthdayChannel.Id,
            ChannelId = birthdayChannel.ChannelId,
            GuildId = birthdayChannel.GuildId,
            JanMessageId = birthdayChannel.JanMessageId,
            FebMessageId = birthdayChannel.FebMessageId,
            MarMessageId = birthdayChannel.MarMessageId,
            AprMessageId = birthdayChannel.AprMessageId,
            MaiMessageId = birthdayChannel.MaiMessageId,
            JunMessageId = birthdayChannel.JunMessageId,
            JulMessageId = birthdayChannel.JulMessageId,
            AugMessageId = birthdayChannel.AugMessageId,
            SepMessageId = birthdayChannel.SepMessageId,
            OctMessageId = birthdayChannel.OctMessageId,
            NovMessageId = birthdayChannel.NovMessageId,
            DezMessageId = birthdayChannel.DezMessageId
        };
        return data;
    }

    public async Task<IEnumerable<BirthdayChannel>> GetAllGeburtstagsChannelAsync()
    {
        var data = await _repository.GetAllGeburtstagsChannelAsync();
        var dtos = data.Select(MapDataToDomain);
        return dtos;
    }

    public async Task<List<Birthday>> GetAllGeburtstageAsync()
    {
        var datas = await _repository.GetAllGeburtstageAsync();
        return datas.Select(MapDataToBirthday).ToList();
    }

    public async Task DeleteBirthdayChannelAsync(long channelId)
    {
        await _repository.DeleteBirthdayChannelAsync(channelId);
    }

    public async Task<bool> HasUserRegisteredBirthday(ulong userId)
    {
        return await _repository.HasUserRegisteredBirthday(userId);
    }

    public async Task SaveBirthdayAsync(Birthday registration)
    {
        var data = MapBirthdayToData(registration);
        await _repository.SaveBirthdayAsync(data);
    }

    public async Task<IEnumerable<BirthdaySubChannel>> GetAllSubbedChannelAsync()
    {
        var datas = await _repository.GetAllSubbedChannelAsync();
        return datas.Select(MapDataToSub);
    }

    public async Task SaveBirthdaySubAsync(BirthdaySubChannel sub)
    {
        var data = new BirthdaySubChannelData
        {
            Id = sub.Id,
            ChannelId = sub.ChannelId,
            GuildId = sub.GuildId
        };
        await _repository.SaveBirthdaySubAsync(data);
    }

    public async Task<bool> IsChannelSubbedAsync(ulong guildId, ulong channelId)
    {
        return await _repository.IsChannelSubbedAsync(guildId, channelId);
    }

    public async Task DeleteSubbedChannelAsync(ulong guildId, ulong channelId)
    {
        await _repository.DeleteSubbedChannelAsync(guildId.ToString(), channelId.ToString());
    }

    public async Task CreateOrUpdateBirthdayRoleAsync(ulong guildId, ulong roleId)
    {
        if (await _repository.HasGuildSetupBirthdayRoleAsync(guildId))
        {
            await _repository.UpdateExistingBirthdayRoleSetupAsync(guildId, roleId);
            return;
        }

        await _repository.CreateNewBirthdayRoleSetupAsync(guildId, roleId);
    }

    public async Task<bool> HasGuildSetupBirthdayRoleAsync(ulong guildId)
    {
        return await _repository.HasGuildSetupBirthdayRoleAsync(guildId);
    }

    public async Task<ulong> RetrieveBirthdayRoleIdForGuildAsync(ulong guildId)
    {
        return await _repository.RetrieveBirthdayRoleIdForGuildAsync(guildId);
    }

    public async Task InsertBirthdayRoleAssotiation(ulong guildId, ulong userId)
    {
        await _repository.InsertBirthdayRoleAssotiation(guildId, userId);
    }

    public async Task<IEnumerable<BirthdayRoleAssotiation>> RetrieveAllBirthdayRoleAssotiations()
    {
        var datas = await _repository.RetrieveAllBirthdayRoleAssotiations();
        return datas.Select(MapAssotiationToDto);
    }

    public async Task<IEnumerable<BirthdayRoleSetup>> RetrieveAllBirthdayRoleSetupsAsync()
    {
        var datas = await _repository.RetrieveAllBirthdaySetupsAsync();
        return datas.Select(MapBirthdayRoleSetupToDto);
    }

    public async Task DeleteAssociationAsync(BirthdayRoleAssotiation birthdayRoleAssociation)
    {
        var id = birthdayRoleAssociation.AssotiationId;
        await _repository.DeleteAssociationAsync(id);
    }

    private BirthdayRoleSetup MapBirthdayRoleSetupToDto(BirthdayRoleSetupData data)
    {
        return new BirthdayRoleSetup
        {
            GuildId = data.GuildId,
            RoleId = data.RoleId,
            SetupId = data.SetupId
        };
    }

    private BirthdayRoleAssotiation MapAssotiationToDto(BirthdayRoleAssotiationData data)
    {
        return new BirthdayRoleAssotiation
        {
            AssotiationId = data.AssotiationId,
            GuildId = data.GuildId,
            UserId = data.UserId
        };
    }

    private BirthdaySubChannel MapDataToSub(BirthdaySubChannelData data)
    {
        return new BirthdaySubChannel
        {
            Id = data.Id,
            ChannelId = data.ChannelId,
            GuildId = data.GuildId
        };
    }

    private BirthdayData MapBirthdayToData(Birthday registration)
    {
        return new BirthdayData
        {
            Geburtsdatum = registration.Geburtsdatum,
            UserId = registration.UserId
        };
    }

    private Birthday MapDataToBirthday(BirthdayData data)
    {
        return new Birthday
        {
            Geburtsdatum = data.Geburtsdatum,
            UserId = data.UserId
        };
    }

    private BirthdayChannel MapDataToDomain(BirthdayChannelData data)
    {
        return new BirthdayChannel
        {
            Id = data.Id,
            ChannelId = data.ChannelId,
            GuildId = data.GuildId,
            JanMessageId = data.JanMessageId,
            FebMessageId = data.FebMessageId,
            MarMessageId = data.MarMessageId,
            AprMessageId = data.AprMessageId,
            MaiMessageId = data.MaiMessageId,
            JunMessageId = data.JunMessageId,
            JulMessageId = data.JulMessageId,
            AugMessageId = data.AugMessageId,
            SepMessageId = data.SepMessageId,
            OctMessageId = data.OctMessageId,
            NovMessageId = data.NovMessageId,
            DezMessageId = data.DezMessageId
        };
    }
}