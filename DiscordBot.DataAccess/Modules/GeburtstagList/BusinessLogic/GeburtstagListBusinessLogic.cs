using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.GeburtstagList;
using DiscordBot.DataAccess.Modules.GeburtstagList.Repository;

namespace DiscordBot.DataAccess.Modules.GeburtstagList.BusinessLogic;

internal class GeburtstagListBusinessLogic : IGeburtstagListBusinessLogic
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
        var data = new BirthdayChannelData(birthdayChannel.Id,
            birthdayChannel.GuildId,
            birthdayChannel.ChannelId,
            birthdayChannel.JanMessageId,
            birthdayChannel.FebMessageId,
            birthdayChannel.MarMessageId,
            birthdayChannel.AprMessageId,
            birthdayChannel.MaiMessageId,
            birthdayChannel.JunMessageId,
            birthdayChannel.JulMessageId,
            birthdayChannel.AugMessageId,
            birthdayChannel.SepMessageId,
            birthdayChannel.OctMessageId,
            birthdayChannel.NovMessageId,
            birthdayChannel.DezMessageId);
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
        var data = new BirthdaySubChannelData(sub.Id, sub.GuildId, sub.ChannelId);
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
            GuildId = ulong.Parse(data.GuildId),
            RoleId = ulong.Parse(data.RoleId),
            SetupId = data.SetupId
        };
    }

    private BirthdayRoleAssotiation MapAssotiationToDto(BirthdayRoleAssotiationData data)
    {
        return new BirthdayRoleAssotiation
        {
            AssotiationId = data.AssotiationId,
            GuildId = ulong.Parse(data.GuildId),
            UserId = ulong.Parse(data.UserId)
        };
    }

    private BirthdaySubChannel MapDataToSub(BirthdaySubChannelData data)
    {
        return new BirthdaySubChannel
        {
            Id = data.Id,
            ChannelId = ulong.Parse(data.ChannelId),
            GuildId = ulong.Parse(data.GuildId)
        };
    }

    private BirthdayData MapBirthdayToData(Birthday registration)
    {
        return new BirthdayData(registration.UserId, registration.Geburtsdatum);
    }

    private Birthday MapDataToBirthday(BirthdayData data)
    {
        return new Birthday
        {
            Geburtsdatum = data.Birthday,
            UserId = ulong.Parse(data.UserId)
        };
    }

    private BirthdayChannel MapDataToDomain(BirthdayChannelData data)
    {
        return new BirthdayChannel
        {
            Id = data.Id,
            ChannelId = ulong.Parse(data.ChannelId),
            GuildId = ulong.Parse(data.GuildId),
            JanMessageId = ulong.Parse(data.JanMessageId),
            FebMessageId = ulong.Parse(data.FebMessageId),
            MarMessageId = ulong.Parse(data.MarMessageId),
            AprMessageId = ulong.Parse(data.AprMessageId),
            MaiMessageId = ulong.Parse(data.MaiMessageId),
            JunMessageId = ulong.Parse(data.JunMessageId),
            JulMessageId = ulong.Parse(data.JulMessageId),
            AugMessageId = ulong.Parse(data.AugMessageId),
            SepMessageId = ulong.Parse(data.SepMessageId),
            OctMessageId = ulong.Parse(data.OctMessageId),
            NovMessageId = ulong.Parse(data.NovMessageId),
            DezMessageId = ulong.Parse(data.DezMessageId)
        };
    }
}