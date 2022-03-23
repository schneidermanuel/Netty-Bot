using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Entities;
using DiscordBot.DataAccess.Modules.GeburtstagList.BusinessLogic;
using DiscordBot.DataAccess.NHibernate;
using NHibernate.Linq;

namespace DiscordBot.DataAccess.Modules.GeburtstagList.Repository;

public class GeburtstagListRepository : IGeburtstagListRepository
{
    private readonly ISessionFactoryProvider _provider;

    public GeburtstagListRepository(ISessionFactoryProvider provider)
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
            ChannelId = data.ChannelId.ToString(),
            GuildId = data.GuildId.ToString(),
            JanMessageId = data.JanMessageId.ToString(),
            FebMessageId = data.FebMessageId.ToString(),
            MarMessageId = data.MarMessageId.ToString(),
            AprMessageId = data.AprMessageId.ToString(),
            MaiMessageId = data.MaiMessageId.ToString(),
            JunMessageId = data.JunMessageId.ToString(),
            JulMessageId = data.JulMessageId.ToString(),
            AugMessageId = data.AugMessageId.ToString(),
            SepMessageId = data.SepMessageId.ToString(),
            OctMessageId = data.OctMessageId.ToString(),
            NovMessageId = data.NovMessageId.ToString(),
            DezMessageId = data.DezMessageId.ToString()
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
                Birthday = data.Geburtsdatum,
                UserId = data.UserId.ToString()
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
                ChannelId = data.ChannelId.ToString(),
                GuildId = data.GuildId.ToString()
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
        return new BirthdayRoleSetupData
        {
            GuildId = ulong.Parse(entity.GuildId),
            RoleId = ulong.Parse(entity.RoleId),
            SetupId = entity.SetupId
        };
    }

    private BirthdayRoleAssotiationData MapAssotiationToData(BirthdayRoleAssotiationEntity entity)
    {
        return new BirthdayRoleAssotiationData
        {
            AssotiationId = entity.AssotiationId,
            GuildId = ulong.Parse(entity.GuildId),
            UserId = ulong.Parse(entity.UserId)
        };
    }

    private BirthdaySubChannelData MapSubToData(BirthdaySubChannelEntity entity)
    {
        return new BirthdaySubChannelData
        {
            Id = entity.Id,
            ChannelId = ulong.Parse(entity.ChannelId),
            GuildId = ulong.Parse(entity.GuildId)
        };
    }

    private BirthdayData MapBirthdayToData(BirthdayEntity entity)
    {
        return new BirthdayData
        {
            UserId = ulong.Parse(entity.UserId),
            Geburtsdatum = entity.Birthday
        };
    }

    private BirthdayChannelData MapToData(BirthdayChannelEntity entity)
    {
        return new BirthdayChannelData
        {
            Id = entity.Id,
            ChannelId = ulong.Parse(entity.ChannelId),
            GuildId = ulong.Parse(entity.GuildId),
            JanMessageId = ulong.Parse(entity.JanMessageId),
            FebMessageId = ulong.Parse(entity.FebMessageId),
            MarMessageId = ulong.Parse(entity.MarMessageId),
            AprMessageId = ulong.Parse(entity.AprMessageId),
            MaiMessageId = ulong.Parse(entity.MaiMessageId),
            JunMessageId = ulong.Parse(entity.JunMessageId),
            JulMessageId = ulong.Parse(entity.JulMessageId),
            AugMessageId = ulong.Parse(entity.AugMessageId),
            SepMessageId = ulong.Parse(entity.SepMessageId),
            OctMessageId = ulong.Parse(entity.OctMessageId),
            NovMessageId = ulong.Parse(entity.NovMessageId),
            DezMessageId = ulong.Parse(entity.DezMessageId),
        };
    }
}