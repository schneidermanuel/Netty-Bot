using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.TwitchNotifications;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.TwitchNotifications;

internal class TwitchNotificationCommands : CommandModuleBase, IGuildModule
{
    private readonly TwitchNotificationsManager _manager;
    private readonly ITwitchNotificationsBusinessLogic _businessLogic;

    public TwitchNotificationCommands(IModuleDataAccess dataAccess, TwitchNotificationsManager manager,
        ITwitchNotificationsBusinessLogic businessLogic) : base(dataAccess)
    {
        _manager = manager;
        _businessLogic = businessLogic;
    }

    public override async Task<bool> CanExecuteAsync(ulong id, SocketCommandContext socketCommandContext)
    {
        await RequirePermissionAsync(socketCommandContext, GuildPermission.Administrator);
        return await IsEnabled(id);
    }

    public override async Task ExecuteAsync(ICommandContext context)
    {
        await ExecuteCommandsAsync(context);
    }

    [Command("registerTwitch")]
    public async Task RegisterTwitchChannel(ICommandContext context)
    {
        var username = await RequireString(context);
        var channelId = context.Channel.Id;
        var guildId = context.Guild.Id;
        var isAlreadyRegistered = await _businessLogic.IsStreamerInGuildAlreadyRegisteredAsync(username, guildId);
        if (isAlreadyRegistered)
        {
            await context.Channel.SendMessageAsync(
                $"Auf diesem Server werden bereits Benachrichtigungen für '{username}' versendet");
            return;
        }

        var message = await RequireReminderOrEmpty(context, 2);
        if (string.IsNullOrEmpty(message.Trim()))
        {
            message = $"{username} hat gerade einen Livestream auf Twitch gestartet!";
        }

        var registration = new TwitchNotificationRegistration
        {
            Message = message,
            Streamer = username,
            ChannelId = channelId,
            GuildId = guildId
        };
        await _businessLogic.SaveRegistrationAsync(registration);
        await context.Channel.SendMessageAsync("Registrierung erfolgreich");
        await _manager.AddUserAsync(registration);
    }

    [Command("unregisterTwitch")]
    public async Task UnregisterTwitchChannel(ICommandContext context)
    {
        var username = await RequireString(context);
        var isRegistered = await _businessLogic.IsStreamerInGuildAlreadyRegisteredAsync(username, context.Guild.Id);
        if (!isRegistered)
        {
            await context.Channel.SendMessageAsync(
                $"Hier werden aktuell keine Benachrichtigungen für '{username}' versendet");
            return;
        }

        await _businessLogic.DeleteRegistrationAsync(username, context.Guild.Id);
        await context.Channel.SendMessageAsync("Benachrichtigungen ausgeschaltet");
        _manager.RemoveUser(username, context.Guild.Id);
    }

    protected override Type RessourceType => typeof(TwitchNotificationRessources);
    public override string ModuleUniqueIdentifier => "TWITCH_NOTIFICATION";
}