using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Entities;
using DiscordBot.DataAccess.Modules.GeburtstagList.Domain;
using DiscordBot.DataAccess.NHibernate;
using NHibernate.Linq;

namespace DiscordBot.DataAccess.Modules.GeburtstagList.Repository;

internal class GeburtstagListRepository : IGeburtstagListRepository
{
    private readonly ISessionProvider _provider;

    public GeburtstagListRepository(ISessionProvider provider)
    {
        _provider = provider;
    }

    public async Task<bool> HasGuildSetupGeburtstagChannelAsync(ulong guildId)
    {
        using (var session = _provider.OpenSession())
        {
            var hasChannel = await session.Query<BirthdayChannelEntity>()
                .Where(x => x.GuildId == guildId.ToString()).AnyAsync();
            return hasChannel;
        }
    }

    public async Task<long> SaveBirthdayChannelAsync(BirthdayChannelData data)
    {
        var entity = new BirthdayChannelEntity
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
        using (var session = _provider.OpenSession())
        {
            var id = await session.SaveAsync(entity);
            await session.FlushAsync();
            return (long)id;
        }
    }

    public async Task<IEnumerable<BirthdayChannelData>> GetAllGeburtstagsChannelAsync()
    {
        using (var session = _provider.OpenSession())
        {
            var entities = await session.Query<BirthdayChannelEntity>().ToListAsync();
            var datas = entities.Select(MapToData);
            return datas;
        }
    }

    public async Task<IEnumerable<BirthdayData>> GetAllGeburtstageAsync()
    {
        using (var session = _provider.OpenSession())
        {
            var birthdays = await session.Query<BirthdayEntity>().ToListAsync();
            return birthdays.Select(MapBirthdayToData);
        }
    }

    public async Task DeleteBirthdayChannelAsync(long channelId)
    {
        using (var session = _provider.OpenSession())
        {
            var entity = await session.LoadAsync<BirthdayChannelEntity>(channelId);
            await session.DeleteAsync(entity);
            await session.FlushAsync();
        }
    }

    public async Task<bool> HasUserRegisteredBirthday(ulong userId)
    {
        using (var session = _provider.OpenSession())
        {
            return await session.Query<BirthdayEntity>()
                .Where(x => x.UserId == userId.ToString())
                .AnyAsync();
        }
    }

    public async Task SaveBirthdayAsync(BirthdayData data)
    {
        using (var session = _provider.OpenSession())
        {
            var entity = new BirthdayEntity
            {
                Birthday = data.Birthday,
                UserId = data.UserId
            };
            await session.SaveAsync(entity);
            await session.FlushAsync();
        }
    }

    public async Task<IEnumerable<BirthdaySubChannelData>> GetAllSubbedChannelAsync()
    {
        using (var session = _provider.OpenSession())
        {
            var entities = await session.Query<BirthdaySubChannelEntity>().ToListAsync();
            return entities.Select(MapSubToData);
        }
    }

    public async Task SaveBirthdaySubAsync(BirthdaySubChannelData data)
    {
        using (var session = _provider.OpenSession())
        {
            var entity = new BirthdaySubChannelEntity
            {
                Id = data.Id,
                ChannelId = data.ChannelId,
                GuildId = data.GuildId
            };
            await session.SaveOrUpdateAsync(entity);
            await session.FlushAsync();
        }
    }

    public async Task<bool> IsChannelSubbedAsync(ulong guildId, ulong channelId)
    {
        using (var session = _provider.OpenSession())
        {
            var results = session.Query<BirthdaySubChannelEntity>()
                .Where(entity => entity.GuildId == guildId.ToString() && entity.ChannelId == channelId.ToString());
            return await results.AnyAsync();
        }
    }

    public async Task DeleteSubbedChannelAsync(string guildId, string channeld)
    {
        using (var session = _provider.OpenSession())
        {
            var result = await session.Query<BirthdaySubChannelEntity>()
                .Where(sub => sub.GuildId == guildId && sub.ChannelId == channeld)
                .ToListAsync();
            foreach (var entity in result)
            {
                await session.DeleteAsync(entity);
            }

            await session.FlushAsync();
        }
    }

    public async Task<bool> HasGuildSetupBirthdayRoleAsync(ulong guildId)
    {
        using (var session = _provider.OpenSession())
        {
            var query = session.Query<BirthdayRoleSetupEntity>()
                .Where(setup => setup.GuildId == guildId.ToString());
            return await query.AnyAsync();
        }
    }

    public async Task UpdateExistingBirthdayRoleSetupAsync(ulong guildId, ulong roleId)
    {
        using (var session = _provider.OpenSession())
        {
            var query = session.Query<BirthdayRoleSetupEntity>()
                .Where(setup => setup.GuildId == guildId.ToString());
            var entity = await query.SingleAsync();
            entity.RoleId = roleId.ToString();
            await session.UpdateAsync(entity);
            await session.FlushAsync();
        }
    }

    public async Task CreateNewBirthdayRoleSetupAsync(ulong guildId, ulong roleId)
    {
        using (var session = _provider.OpenSession())
        {
            var entity = new BirthdayRoleSetupEntity
            {
                SetupId = 0,
                GuildId = guildId.ToString(),
                RoleId = roleId.ToString()
            };
            await session.SaveAsync(entity);
            await session.FlushAsync();
        }
    }

    public async Task<ulong> RetrieveBirthdayRoleIdForGuildAsync(ulong guildId)
    {
        using (var session = _provider.OpenSession())
        {
            var entity = await session.Query<BirthdayRoleSetupEntity>()
                .Where(setup => setup.GuildId == guildId.ToString())
                .SingleAsync();
            return ulong.Parse(entity.RoleId);
        }
    }

    public async Task InsertBirthdayRoleAssotiation(ulong guildId, ulong userId)
    {
        using (var session = _provider.OpenSession())
        {
            var entity = new BirthdayRoleAssotiationEntity
            {
                AssotiationId = 0,
                GuildId = guildId.ToString(),
                UserId = userId.ToString()
            };
            await session.SaveAsync(entity);
            await session.FlushAsync();
        }
    }

    public async Task<IEnumerable<BirthdayRoleAssotiationData>> RetrieveAllBirthdayRoleAssotiations()
    {
        using (var session = _provider.OpenSession())
        {
            var query = session.Query<BirthdayRoleAssotiationEntity>();
            var entities = await query.ToListAsync();
            return entities.Select(MapAssotiationToData);
        }
    }

    public async Task<IEnumerable<BirthdayRoleSetupData>> RetrieveAllBirthdaySetupsAsync()
    {
        using (var session = _provider.OpenSession())
        {
            var entities = await session.Query<BirthdayRoleSetupEntity>().ToListAsync();
            return entities.Select(MapSetupToData);
        }
    }

    public async Task DeleteAssociationAsync(long id)
    {
        using (var session = _provider.OpenSession())
        {
            var entity = await session.LoadAsync<BirthdayRoleAssotiationEntity>(id);
            await session.DeleteAsync(entity);
            await session.FlushAsync();
        }
    }

    private BirthdayRoleSetupData MapSetupToData(BirthdayRoleSetupEntity entity)
    {
        return new BirthdayRoleSetupData(entity.SetupId, entity.GuildId, entity.RoleId);
    }

    private BirthdayRoleAssotiationData MapAssotiationToData(BirthdayRoleAssotiationEntity entity)
    {
        return new BirthdayRoleAssotiationData(entity.AssotiationId, entity.GuildId, entity.UserId);
    }

    private BirthdaySubChannelData MapSubToData(BirthdaySubChannelEntity entity)
    {
        return new BirthdaySubChannelData(entity.Id, entity.GuildId, entity.ChannelId);
    }

    private BirthdayData MapBirthdayToData(BirthdayEntity entity)
    {
        return new BirthdayData(entity.UserId, entity.Birthday);
    }

    private BirthdayChannelData MapToData(BirthdayChannelEntity entity)
    {
        return new BirthdayChannelData(
            entity.Id,
            entity.GuildId,
            entity.ChannelId,
            entity.JanMessageId,
            entity.FebMessageId,
            entity.MarMessageId,
            entity.AprMessageId,
            entity.MaiMessageId,
            entity.JunMessageId,
            entity.JulMessageId,
            entity.AugMessageId,
            entity.SepMessageId,
            entity.OctMessageId,
            entity.NovMessageId,
            entity.DezMessageId);
    }
}