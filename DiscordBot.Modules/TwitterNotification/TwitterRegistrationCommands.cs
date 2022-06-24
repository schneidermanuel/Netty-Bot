using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.TwitterRegistration;
using DiscordBot.DataAccess.Contract.TwitterRegistration.BusinessLogic;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.TwitterNotification;

internal class TwitterRegistrationCommands : CommandModuleBase, IGuildModule
{
    private readonly ITwitterRegistrationBusinessLogic _businessLogic;
    private readonly TwitterStreamManager _manager;
    private readonly TwitterRuleValidator _ruleValidator;

    public TwitterRegistrationCommands(IModuleDataAccess dataAccess, ITwitterRegistrationBusinessLogic businessLogic,
        TwitterStreamManager manager, TwitterRuleValidator ruleValidator) :
        base(dataAccess)
    {
        _businessLogic = businessLogic;
        _manager = manager;
        _ruleValidator = ruleValidator;
    }

    protected override Type RessourceType => typeof(TwitterNotificationRessources);

    public override async Task<bool> CanExecuteAsync(ulong id, SocketCommandContext socketCommandContext)
    {
        await RequirePermissionAsync(socketCommandContext, GuildPermission.Administrator);
        return await IsEnabled(id);
    }

    public override async Task ExecuteAsync(ICommandContext context)
    {
        await ExecuteCommandsAsync(context);
    }

    [Command("registerTwitter")]
    public async Task RegisterTwitterAsync(ICommandContext context)
    {
        var username = await RequireString(context);
        if (await _businessLogic.IsAccountRegisteredOnChannelAsync(context.Guild.Id, context.Channel.Id, username))
        {
            await context.Channel.SendMessageAsync(
                Localize(nameof(TwitterNotificationRessources.Error_AccountAlreadyRegistered)));
            return;
        }

        var arg = await RequireReminderOrEmpty(context, 2);
        var message = ExtractMessage(arg, username);
        var rule = ExtractRule(arg);

        if (!_ruleValidator.IsRuleValid(rule))
        {
            await context.Channel.SendMessageAsync(Localize(nameof(TwitterNotificationRessources.Error_InvalidRule)));
            return;
        }

        var dto = new TwitterRegistrationDto
        {
            Message = message,
            Username = username,
            ChannelId = context.Channel.Id,
            GuildId = context.Guild.Id,
            RuleFilter = rule
        };

        await _businessLogic.RegisterTwitterAsync(dto);
        await context.Channel.SendMessageAsync(Localize(nameof(TwitterNotificationRessources.Message_Registered)));
        await _manager.RegisterTwitterUserAsync(dto);
    }

    [Command("unregisterTwitter")]
    public async Task UnregisterAsync(SocketCommandContext context)
    {
        var username = await RequireString(context);
        if (!await _businessLogic.IsAccountRegisteredOnChannelAsync(context.Guild.Id, context.Channel.Id, username))
        {
            await context.Channel.SendMessageAsync(
                Localize(nameof(TwitterNotificationRessources.Error_AccountNotRegistered)));
            return;
        }
        await _businessLogic.UnregisterTwitterAsync(context.Guild.Id, context.Channel.Id, username);
    }
    

    private string ExtractRule(string arg)
    {
        return arg.Contains(';') ? arg.Split(';')[1] : string.Empty;
    }

    private string ExtractMessage(string arg, string username)
    {
        var message = arg.Contains(';') ? arg.Split(';')[0] : arg;
        return string.IsNullOrEmpty(message)
            ? string.Format(Localize(nameof(TwitterNotificationRessources.Message_Default)), username)
            : message;
    }


    public override string ModuleUniqueIdentifier => "TWITTER_NOTIFICATIONS";
}