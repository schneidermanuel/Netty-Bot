using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.GeburtstagList;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.BirthdayList;

public class BirthdayCommands : CommandModuleBase, ICommandModule
{
    private readonly IModuleDataAccess _dataAccess;
    private readonly IGeburtstagListDomain _domain;
    private readonly BirthdayListManager _manager;

    public BirthdayCommands(IModuleDataAccess dataAccess, IGeburtstagListDomain domain,
        BirthdayListManager manager) : base(dataAccess)
    {
        _dataAccess = dataAccess;
        _domain = domain;
        _manager = manager;
    }

    protected override Type RessourceType => typeof(BirthdayListRessources);
    public override string ModuleUniqueIdentifier => "BIRTHDAYLIST";


    [Command("registerBirthday")]
    [Description("Saves your birthday to the birthday list")]
    [Parameter(Name = "birthday", Description = "Your birthday in the format dd.MM (31.01)", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.String)]
    public async Task RegisterBirthday(SocketSlashCommand context)
    {
        var hasUserBirthdayRegistered = await _domain.HasUserRegisteredBirthday(context.User.Id);
        if (hasUserBirthdayRegistered)
        {
            await context.RespondAsync(Localize(nameof(BirthdayListRessources.Error_BirthdayAlreadySaved)));
            return;
        }
        var guild = await RequireGuild(context);

        var prefix = await _dataAccess.GetServerPrefixAsync(guild.Id);
        try
        {
            var date = await RequireString(context);
            var day = int.Parse(date.Split(".")[0]);
            var month = int.Parse(date.Split(".")[1]);
            var birthday = new DateTime(2000, month, day);
            var registration = new Birthday
            {
                Geburtsdatum = birthday,
                UserId = context.User.Id
            };
            await _domain.SaveBirthdayAsync(registration);
            await context.RespondAsync(Localize(nameof(BirthdayListRessources.Message_BirthdaySaved)));
            await _manager.RefreshAllBirthdayChannel();
        }
        catch (Exception)
        {
            await context.RespondAsync(string.Format(Localize(nameof(BirthdayListRessources.Error_InvalidFormat)),
                prefix));
        }
    }

    [Command("registerBirthdayChannel")]
    [Description("Lists the birthday of all members in this channel")]
    public async Task RegisterBirthdayChannel(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);
        await RequirePermissionAsync(context, guild, GuildPermission.Administrator);

        var hasGuildChannel = await _domain.HasGuildSetupGeburtstagChannelAsync(guild.Id);
        if (hasGuildChannel)
        {
            await context.RespondAsync(
                Localize(nameof(BirthdayListRessources.Error_BirthdaylistAlreadyPresent)));
            return;
        }

        var channel = context.Channel;
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
            GuildId = guild.Id,
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
        var id = await _domain.SaveBirthdayChannelAsync(birthdayChannel);
        birthdayChannel.Id = id;
        await _manager.RefreshSingleBirthdayChannel(birthdayChannel);
    }

    [Command("unsubBirthdays")]
    [Description("Stops sending Informations on birthdays in this channel")]
    public async Task UbsubChannelFromBirthdayEvents(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);
        await RequirePermissionAsync(context, guild, GuildPermission.Administrator);

        var channel = context.Channel;
        var guildId = guild.Id;

        var isChannelSubbed = await _domain.IsChannelSubbedAsync(guildId, channel.Id);
        if (!isChannelSubbed)
        {
            await context.RespondAsync(
                Localize(nameof(BirthdayListRessources.Error_NotificationsNotEnabled)));
            return;
        }

        await _domain.DeleteSubbedChannelAsync(guildId, channel.Id);
        await context.RespondAsync("🤝");
    }

    [Command("subBirthdays")]
    [Description("Starts sending Informations on birthdays in this channel")]
    public async Task SubChannelToBirthdayEvents(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);
        await RequirePermissionAsync(context, guild, GuildPermission.Administrator);

        var allSubbedChannel = await _domain.GetAllSubbedChannelAsync();
        var channel = context.Channel;
        var guildId = guild.Id;
        if (allSubbedChannel.Any(sub => sub.GuildId == guildId))
        {
            await context.RespondAsync(
                Localize(nameof(BirthdayListRessources.Error_NotificationsAlreadyEnabled)));
            return;
        }

        var sub = new BirthdaySubChannel
        {
            Id = 0,
            ChannelId = channel.Id,
            GuildId = guildId
        };
        await _domain.SaveBirthdaySubAsync(sub);
        await context.RespondAsync(
            Localize(nameof(BirthdayListRessources.Message_BirthdayNotificationsEnabled)));
    }

    [Command("setBirthdayRole")]
    [Description("Sets the role that will be given to the user at their birthday")]
    [Parameter(Name = "role", Description = "The role that will be given to the user at their birthday", IsOptional = false, ParameterType = ApplicationCommandOptionType.Role)]
    public async Task SetupBirthdayRoleCommandAsync(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);
        await RequirePermissionAsync(context, guild, GuildPermission.Administrator);

        var role = await RequireRoleAsync(context);

        if (!await _domain.HasGuildSetupGeburtstagChannelAsync(guild.Id))
        {
            await context.RespondAsync(Localize(nameof(BirthdayListRessources.Error_NoBirthdayChannel)));
            return;
        }

        if (role == null)
        {
            await context.RespondAsync(
                string.Format(Localize(nameof(BirthdayListRessources.Error_RoleNotFound))));
            return;
        }

        await _domain.CreateOrUpdateBirthdayRoleAsync(guild.Id, role.Id);
        await context.RespondAsync(
            string.Format(Localize(nameof(BirthdayListRessources.Message_BirthdayRoleSuccess)), role.Name));
    }
}