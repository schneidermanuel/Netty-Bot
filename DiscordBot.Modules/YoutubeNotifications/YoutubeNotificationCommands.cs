using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.YoutubeNotification;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.YoutubeNotifications;

internal class YoutubeNotificationCommands : CommandModuleBase, ICommandModule
{
    private readonly IYoutubeNotificationBusinessLogic _businessLogic;
    private readonly YoutubeNotificationManager _manager;

    public YoutubeNotificationCommands(IModuleDataAccess dataAccess, IYoutubeNotificationBusinessLogic businessLogic,
        YoutubeNotificationManager manager) :
        base(dataAccess)
    {
        _businessLogic = businessLogic;
        _manager = manager;
    }
    

    protected override Type RessourceType => typeof(YoutubeNotificationRessources);
    public override string ModuleUniqueIdentifier => "YOUTUBE_NOTIFICATIONS";

    [Command("registerYoutube")]
    [Description("Register a channel to receive youtube notifications")]
    [Parameter(Name = "channelid", Description = "The channel-id of the channel to register", IsOptional = false, ParameterType = ApplicationCommandOptionType.String)]
    [Parameter(Name = "message", Description = "The message to send when a video is uploaded", IsOptional = true, ParameterType = ApplicationCommandOptionType.String)]
    public async Task RegisterYoutubeAsync(SocketSlashCommand context, IGuild guild)
    {
        await RequirePermissionAsync(context, guild, GuildPermission.Administrator);
        var youtubeChannelId = await RequireString(context);
        var guildId = guild.Id;
        var channelId = context.Channel.Id;
        var message = RequireStringOrEmpty(context, 2).Trim();

        var isAlreadyRegistered =
            await _businessLogic.IsStreamerInGuildAlreadyRegisteredAsync(youtubeChannelId, guildId);
        if (isAlreadyRegistered)
        {
            await context.RespondAsync(Localize(nameof(YoutubeNotificationRessources.Error_AlreadyRegistered)));
            return;
        }

        var username = _manager.GetUsernameById(youtubeChannelId);
        if (username == null)
        {
            await context.RespondAsync(Localize(nameof(YoutubeNotificationRessources.Error_InvalidChannel)));
            return;
        }

        if (string.IsNullOrEmpty(message))
        {
            message = string.Format(Localize(nameof(YoutubeNotificationRessources.Message_NewVideo)), username);
        }


        var registration = new YoutubeNotificationRegistration
        {
            Message = message,
            ChannelId = channelId,
            GuildId = guildId,
            YoutubeChannelId = youtubeChannelId
        };
        var id = await _businessLogic.SaveRegistrationAsync(registration);
        registration.RegistrationId = id;
        await _manager.RegisterChannelAsync(registration);
        await context.RespondAsync(Localize(nameof(YoutubeNotificationRessources.Message_RegistrationSuccess)));
    }

    [Command("unregisterYoutube")]
    [Description("Unregister a channel from receiving youtube notifications")]
    [Parameter(Name = "channelid", Description = "The channel-id of the channel to unregister", IsOptional = false, ParameterType = ApplicationCommandOptionType.String)]
    public async Task UnregisterYoutubeAsync(SocketSlashCommand context, IGuild guild)
    {
        var youtubeChannelId = await RequireString(context);
        var guildId = guild.Id;
        var isAlreadyRegistered =
            await _businessLogic.IsStreamerInGuildAlreadyRegisteredAsync(youtubeChannelId, guildId);
        if (!isAlreadyRegistered)
        {
            await context.RespondAsync(Localize(nameof(YoutubeNotificationRessources.Error_InvalidRegistration)));
            return;
        }

        await _businessLogic.DeleteRegistrationAsync(youtubeChannelId, guildId);
        await context.RespondAsync(Localize(nameof(YoutubeNotificationRessources.Message_UnregistrationSuccss)));
        _manager.RemoveRegistration(youtubeChannelId, guildId);
    }
}