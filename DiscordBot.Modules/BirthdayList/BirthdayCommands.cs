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
            await context.Channel.SendMessageAsync("Dein Geburtstag ist bereits in der Datenbank vorhanden. ");
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
            await context.Channel.SendMessageAsync("Geburtstag registriert!");
            await _manager.RefreshAllBirthdayChannel((DiscordSocketClient)context.Client);
        }
        catch (Exception)
        {
            await context.Channel.SendMessageAsync(
                $"Bitte den Geburtstag im Format {prefix}registerBirthday dd.MM");
        }
    }

    [Command("registerBirthdayChannel")]
    public async Task RegisterBirthdayChannel(ICommandContext context)
    {
        await RequirePermissionAsync(context, GuildPermission.Administrator);

        var hasGuildChannel = await _businessLogic.HasGuildSetupGeburtstagChannelAsync(context.Guild.Id);
        if (hasGuildChannel)
        {
            await context.Channel.SendMessageAsync(
                "Auf diesem Server ist bereits eine Geburtstagsliste vorhanden. ");
            return;
        }

        var channel = (ISocketMessageChannel)context.Channel;
        await channel.SendMessageAsync("**🍰GEBURTSTAGSLISTE🍰**");
        var janMsg = await channel.SendMessageAsync("Januar:");
        var febMsg = await channel.SendMessageAsync("Februar:");
        var marMsg = await channel.SendMessageAsync("März:");
        var aprMsg = await channel.SendMessageAsync("April:");
        var maiMsg = await channel.SendMessageAsync("Mai:");
        var junMsg = await channel.SendMessageAsync("Juni:");
        var julMsg = await channel.SendMessageAsync("Juli:");
        var augMsg = await channel.SendMessageAsync("August:");
        var sepMsg = await channel.SendMessageAsync("September:");
        var octMsg = await channel.SendMessageAsync("Oktober:");
        var novMsg = await channel.SendMessageAsync("Novemver:");
        var dezMsg = await channel.SendMessageAsync("Dezember:");
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
            await context.Channel.SendMessageAsync("Kanal ist nicht registriert. ");
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
            await context.Channel.SendMessageAsync(
                "Auf diesem Server sind bereits Geburtstagsbenachrichtigungen aktiv.");
            return;
        }

        var sub = new BirthdaySubChannel
        {
            Id = 0,
            ChannelId = channel.Id,
            GuildId = guildId
        };
        await _businessLogic.SaveBirthdaySubAsync(sub);
        await context.Channel.SendMessageAsync("Geburtstagsbenachrichtigungen wurden für diesen Kanal aktiviert!");
    }

    [Command("setBirthdayRole")]
    public async Task SetupBirthdayRoleCommandAsync(ICommandContext context)
    {
        await RequirePermissionAsync(context, GuildPermission.Administrator);

        var roleId = await RequireUlongAsync(context);

        if (!await _businessLogic.HasGuildSetupGeburtstagChannelAsync(context.Guild.Id))
        {
            await context.Channel.SendMessageAsync("Für diesen Server ist kein Geburtstagskanal eingetragen.");
            return;
        }

        var role = context.Guild.GetRole(roleId);
        if (role == null)
        {
            await context.Channel.SendMessageAsync(
                $"Für die ID '{roleId}' wurde keine Rolle auf dem Server '{context.Guild.Name}' gefunden");
            return;
        }

        await _businessLogic.CreateOrUpdateBirthdayRoleAsync(context.Guild.Id, roleId);
        await context.Channel.SendMessageAsync(
            $"Die Rolle '{role.Name}' wird nun an Geburtstagen automatisch verteilt!\nDie Rolle wird jeweils um 05:00 MEZ verteilt und entfernt.");
    }
}