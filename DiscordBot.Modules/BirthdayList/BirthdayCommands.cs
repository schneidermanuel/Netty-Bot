using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.GeburtstagList;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.BirthdayList;

public class BirthdayCommands : CommandModuleBase, IGuildModule
{
    private readonly IModuleDataAccess _dataAccess;
    private readonly IGeburtstagListBusinessLogic _businessLogic;
    private readonly BirthdayListManager _manager;

    public BirthdayCommands(IModuleDataAccess dataAccess, IGeburtstagListBusinessLogic businessLogic,
        BirthdayListManager manager) : base(dataAccess)
    {
        _dataAccess = dataAccess;
        _businessLogic = businessLogic;
        _manager = manager;
    }

    protected override Type RessourceType => typeof(BirthdayListRessources);
    public override string ModuleUniqueIdentifier => "BIRTHDAYLIST";

    public override async Task<bool> CanExecuteAsync(ulong id, SocketCommandContext socketCommandContext)
    {
        return await IsEnabled(id);
    }

    public override async Task ExecuteAsync(ICommandContext context)
    {
        await ExecuteCommandsAsync(context);
    }

    [Command("registerBirthday")]
    public async Task RegisterBirthday(ICommandContext context)
    {
        var hasUserBirthdayRegistered = await _businessLogic.HasUserRegisteredBirthday(context.User.Id);
        if (hasUserBirthdayRegistered)
        {
            await context.Channel.SendMessageAsync(Localize(nameof(BirthdayListRessources.Error_BirthdayAlreadySaved)));
            return;
        }

        var prefix = await _dataAccess.GetServerPrefixAsync(context.Guild.Id);
        try
        {
            var date = context.Message.Content.Split(" ")[1];
            var day = int.Parse(date.Split(".")[0]);
            var month = int.Parse(date.Split(".")[1]);
            var birthday = new DateTime(2000, month, day);
            var registration = new Birthday
            {
                Geburtsdatum = birthday,
                UserId = context.User.Id
            };
            await _businessLogic.SaveBirthdayAsync(registration);
            await context.Channel.SendMessageAsync(Localize(nameof(BirthdayListRessources.Message_BirthdaySaved)));
            await _manager.RefreshAllBirthdayChannel((DiscordSocketClient)context.Client);
        }
        catch (Exception)
        {
            await context.Channel.SendMessageAsync(string.Format(Localize(nameof(BirthdayListRessources.Error_InvalidFormat)), prefix));
        }
    }

    [Command("registerBirthdayChannel")]
    public async Task RegisterBirthdayChannel(ICommandContext context)
    {
        await RequirePermissionAsync(context, GuildPermission.Administrator);

        var hasGuildChannel = await _businessLogic.HasGuildSetupGeburtstagChannelAsync(context.Guild.Id);
        if (hasGuildChannel)
        {
            await context.Channel.SendMessageAsync(Localize(nameof(BirthdayListRessources.Error_BirthdaylistAlreadyPresent)));
            return;
        }

        var channel = (ISocketMessageChannel)context.Channel;
        await channel.SendMessageAsync(Localize(nameof(BirthdayListRessources.Title_Birthdaylist)));
        var janMsg = await channel.SendMessageAsync(Localize(nameof(BirthdayListRessources.Month_1)));
        var febMsg = await channel.SendMessageAsync(Localize(nameof(BirthdayListRessources.Month_2)));
        var marMsg = await channel.SendMessageAsync(Localize(nameof(BirthdayListRessources.Month_3)));
        var aprMsg = await channel.SendMessageAsync(Localize(nameof(BirthdayListRessources.Month_4)));
        var maiMsg = await channel.SendMessageAsync(Localize(nameof(BirthdayListRessources.Month_5)));
        var junMsg = await channel.SendMessageAsync(Localize(nameof(BirthdayListRessources.Month_6)));
        var julMsg = await channel.SendMessageAsync(Localize(nameof(BirthdayListRessources.Month_7)));
        var augMsg = await channel.SendMessageAsync(Localize(nameof(BirthdayListRessources.Month_8)));
        var sepMsg = await channel.SendMessageAsync(Localize(nameof(BirthdayListRessources.Month_9)));
        var octMsg = await channel.SendMessageAsync(Localize(nameof(BirthdayListRessources.Month_10)));
        var novMsg = await channel.SendMessageAsync(Localize(nameof(BirthdayListRessources.Month_11)));
        var dezMsg = await channel.SendMessageAsync(Localize(nameof(BirthdayListRessources.Month_12)));
        var birthdayChannel = new BirthdayChannel
        {
            Id = 0,
            ChannelId = channel.Id,
            GuildId = context.Guild.Id,
            JanMessageId = janMsg.Id,
            FebMessageId = febMsg.Id,
            MarMessageId = marMsg.Id,
            AprMessageId = aprMsg.Id,
            MaiMessageId = maiMsg.Id,
            JunMessageId = junMsg.Id,
            JulMessageId = julMsg.Id,
            AugMessageId = augMsg.Id,
            SepMessageId = sepMsg.Id,
            OctMessageId = octMsg.Id,
            NovMessageId = novMsg.Id,
            DezMessageId = dezMsg.Id
        };
        var id = await _businessLogic.SaveBirthdayChannelAsync(birthdayChannel);
        birthdayChannel.Id = id;
        await _manager.RefreshSingleBirthdayChannel(birthdayChannel, (DiscordSocketClient)context.Client);
    }

    [Command("unsubBirthdays")]
    public async Task UbsubChannelFromBirthdayEvents(ICommandContext context)
    {
        await RequirePermissionAsync(context, GuildPermission.Administrator);

        var channel = context.Channel;
        var guildId = context.Guild.Id;

        var isChannelSubbed = await _businessLogic.IsChannelSubbedAsync(guildId, channel.Id);
        if (!isChannelSubbed)
        {
            await context.Channel.SendMessageAsync(Localize(nameof(BirthdayListRessources.Error_NotificationsNotEnabled)));
            return;
        }

        await _businessLogic.DeleteSubbedChannelAsync(guildId, channel.Id);
    }

    [Command("subBirthdays")]
    public async Task SubChannelToBirthdayEvents(ICommandContext context)
    {
        await RequirePermissionAsync(context, GuildPermission.Administrator);

        var allSubbedChannel = await _businessLogic.GetAllSubbedChannelAsync();
        var channel = context.Channel;
        var guildId = context.Guild.Id;
        if (allSubbedChannel.Any(sub => sub.GuildId == guildId))
        {
            await context.Channel.SendMessageAsync(Localize(nameof(BirthdayListRessources.Error_NotificationsAlreadyEnabled)));
            return;
        }

        var sub = new BirthdaySubChannel
        {
            Id = 0,
            ChannelId = channel.Id,
            GuildId = guildId
        };
        await _businessLogic.SaveBirthdaySubAsync(sub);
        await context.Channel.SendMessageAsync(Localize(nameof(BirthdayListRessources.Message_BirthdayNotificationsEnabled)));
    }

    [Command("setBirthdayRole")]
    public async Task SetupBirthdayRoleCommandAsync(ICommandContext context)
    {
        await RequirePermissionAsync(context, GuildPermission.Administrator);

        var roleId = await RequireUlongAsync(context);

        if (!await _businessLogic.HasGuildSetupGeburtstagChannelAsync(context.Guild.Id))
        {
            await context.Channel.SendMessageAsync(Localize(nameof(BirthdayListRessources.Error_NoBirthdayChannel)));
            return;
        }

        var role = context.Guild.GetRole(roleId);
        if (role == null)
        {
            await context.Channel.SendMessageAsync(string.Format(Localize(nameof(BirthdayListRessources.Error_RoleNotFound)), roleId));
            return;
        }

        await _businessLogic.CreateOrUpdateBirthdayRoleAsync(context.Guild.Id, roleId);
        await context.Channel.SendMessageAsync(string.Format(Localize(nameof(BirthdayListRessources.Message_BirthdayRoleSuccess)), role.Name));
    }
}