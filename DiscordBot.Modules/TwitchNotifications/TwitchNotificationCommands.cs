using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.TwitchNotifications;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.TwitchNotifications;

internal class TwitchNotificationCommands : CommandModuleBase, ICommandModule
{
    private readonly TwitchNotificationsManager _manager;
    private readonly ITwitchNotificationsDomain _domain;

    public TwitchNotificationCommands(IModuleDataAccess dataAccess, TwitchNotificationsManager manager,
        ITwitchNotificationsDomain domain) : base(dataAccess)
    {
        _manager = manager;
        _domain = domain;
    }

    [Command("registerTwitch")]
    [Description("Sends notification whenever a twitch stream is live")]
    [Parameter(Name = "username", Description = "The twitch username", IsOptional = false, ParameterType = ApplicationCommandOptionType.String)]
    [Parameter(Name = "message", Description = "The message sent when the stream starts", IsOptional = true, ParameterType = ApplicationCommandOptionType.String)]
    public async Task RegisterTwitchChannel(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);
        await RequirePermissionAsync(context, guild, GuildPermission.Administrator);
        var username = await RequireString(context);
        var channelId = context.Channel.Id;
        var guildId = guild.Id;
        var isAlreadyRegistered = await _domain.IsStreamerInGuildAlreadyRegisteredAsync(username, guildId);
        if (isAlreadyRegistered)
        {
            await context.RespondAsync(Localize(nameof(TwitchNotificationRessources.Message_RegistrationSuccess)));
            return;
        }

        var message = RequireStringOrEmpty(context, 2);
        if (string.IsNullOrEmpty(message.Trim()))
        {
            message = string.Format(Localize(nameof(TwitchNotificationRessources.Message_NewStream)), username);
        }
        
        var registration = new TwitchNotificationRegistration
        {
            Message = message,
            Streamer = username,
            ChannelId = channelId,
            GuildId = guildId
        };
        await _domain.SaveRegistrationAsync(registration);
        await context.RespondAsync(Localize(nameof(TwitchNotificationRessources.Message_RegistrationSuccess)));
        await _manager.AddUserAsync(registration);
    }

    [Command("unregisterTwitch")]
    [Description("Unregisters a twitch channel from the notification service")]
    [Parameter(Name = "username", Description = "The twitch username", IsOptional = false, ParameterType = ApplicationCommandOptionType.String)]
    public async Task UnregisterTwitchChannel(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);
        await RequirePermissionAsync(context, guild, GuildPermission.Administrator);
        var username = await RequireString(context);
        var isRegistered = await _domain.IsStreamerInGuildAlreadyRegisteredAsync(username, guild.Id);
        if (!isRegistered)
        {
            await context.RespondAsync(Localize(nameof(TwitchNotificationRessources.Error_RegistrationInvalid)));
            return;
        }

        await _domain.DeleteRegistrationAsync(username, guild.Id);
        await context.RespondAsync(Localize(nameof(TwitchNotificationRessources.Message_UnregistrationSucess)));
        _manager.RemoveUser(username, guild.Id);
    }

    protected override Type RessourceType => typeof(TwitchNotificationRessources);
    public override string ModuleUniqueIdentifier => "TWITCH_NOTIFICATION";
}