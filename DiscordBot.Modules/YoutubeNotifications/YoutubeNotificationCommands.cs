using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.YoutubeNotification;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.YoutubeNotifications;

internal class YoutubeNotificationCommands : CommandModuleBase, IGuildModule
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

    public override async Task<bool> CanExecuteAsync(ulong id, SocketCommandContext socketCommandContext)
    {
        if (socketCommandContext.User is SocketGuildUser guildUser)
        {
            return await IsEnabled(id) && guildUser.GuildPermissions.Administrator;
        }

        return await IsEnabled(id);
    }

    public override async Task ExecuteAsync(ICommandContext context)
    {
        await ExecuteCommandsAsync(context);
    }

    protected override Type RessourceType => typeof(YoutubeNotificationRessources);
    public override string ModuleUniqueIdentifier => "YOUTUBE_NOTIFICATIONS";

    [Command("registerYoutube")]
    public async Task RegisterYoutubeAsync(ICommandContext context)
    {
        var youtubeChannelId = await RequireString(context);
        var guildId = context.Guild.Id;
        var channelId = context.Channel.Id;
        var message = (await RequireReminderOrEmpty(context, 2)).Trim();

        var isAlreadyRegistered =
            await _businessLogic.IsStreamerInGuildAlreadyRegisteredAsync(youtubeChannelId, guildId);
        if (isAlreadyRegistered)
        {
            await context.Channel.SendMessageAsync(Localize(nameof(YoutubeNotificationRessources.Error_AlreadyRegistered)));
            return;
        }

        var username = _manager.GetUsernameById(youtubeChannelId);
        if (username == null)
        {
            await context.Channel.SendMessageAsync(Localize(nameof(YoutubeNotificationRessources.Error_InvalidChannel)));
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
        await context.Channel.SendMessageAsync(Localize(nameof(YoutubeNotificationRessources.Message_RegistrationSuccess)));
    }

    [Command("unregisterYoutube")]
    public async Task UnregisterYoutubeAsync(ICommandContext context)
    {
        var youtubeChannelId = await RequireString(context);
        var guildId = context.Guild.Id;
        var isAlreadyRegistered =
            await _businessLogic.IsStreamerInGuildAlreadyRegisteredAsync(youtubeChannelId, guildId);
        if (!isAlreadyRegistered)
        {
            await context.Channel.SendMessageAsync(Localize(nameof(YoutubeNotificationRessources.Error_InvalidRegistration)));
            return;
        }

        await _businessLogic.DeleteRegistrationAsync(youtubeChannelId, guildId);
        await context.Channel.SendMessageAsync(Localize(nameof(YoutubeNotificationRessources.Message_UnregistrationSuccss)));
        _manager.RemoveRegistration(youtubeChannelId, guildId);
    }
}