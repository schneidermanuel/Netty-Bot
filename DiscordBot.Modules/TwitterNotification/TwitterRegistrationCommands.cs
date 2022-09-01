using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.TwitterRegistration;
using DiscordBot.DataAccess.Contract.TwitterRegistration.BusinessLogic;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.TwitterNotification;

internal class TwitterRegistrationCommands : CommandModuleBase, ICommandModule
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


    [Command("registerTwitter")]
    [Description("Sends a message whenever a twitter user posts a tweet")]
    [Parameter(Name = "username", Description = "The twitter username to register", IsOptional = false, ParameterType = ApplicationCommandOptionType.String)]
    [Parameter(Name = "message", Description = "The message to send", IsOptional = true, ParameterType = ApplicationCommandOptionType.String)]
    [Parameter(Name = "rule", Description = "The rule to determine if a tweet should be postet", IsOptional = true, ParameterType = ApplicationCommandOptionType.String)]
    public async Task RegisterTwitterAsync(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);
        await RequirePermissionAsync(context, guild, GuildPermission.Administrator);
        var username = await RequireString(context);
        if (await _businessLogic.IsAccountRegisteredOnChannelAsync(guild.Id, context.Channel.Id, username))
        {
            await context.RespondAsync(
                Localize(nameof(TwitterNotificationRessources.Error_AccountAlreadyRegistered)));
            return;
        }

        var message = await RequireString(context, 2);
        var rule = await RequireString(context, 3);
        
        if (!_ruleValidator.IsRuleValid(rule))
        {
            await context.RespondAsync(Localize(nameof(TwitterNotificationRessources.Error_InvalidRule)));
            return;
        }

        var dto = new TwitterRegistrationDto
        {
            Message = message,
            Username = username,
            ChannelId = context.Channel.Id,
            GuildId = guild.Id,
            RuleFilter = rule
        };

        await _businessLogic.RegisterTwitterAsync(dto);
        await context.RespondAsync(Localize(nameof(TwitterNotificationRessources.Message_Registered)));
        await _manager.RegisterTwitterUserAsync(dto);
    }

    [Command("unregisterTwitter")]
    [Description("Unregisters a twitter user from the current channel")]
    [Parameter(Name = "username", Description = "The twitter username to unregister", IsOptional = false, ParameterType = ApplicationCommandOptionType.String)]
    public async Task UnregisterAsync(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);
        await RequirePermissionAsync(context, guild, GuildPermission.Administrator);
        var username = await RequireString(context);
        if (!await _businessLogic.IsAccountRegisteredOnChannelAsync(guild.Id, context.Channel.Id, username))
        {
            await context.RespondAsync(
                Localize(nameof(TwitterNotificationRessources.Error_AccountNotRegistered)));
            return;
        }
        await _businessLogic.UnregisterTwitterAsync(guild.Id, context.Channel.Id, username);
        await context.RespondAsync("🤝");
    }
    

    public override string ModuleUniqueIdentifier => "TWITTER_NOTIFICATIONS";
}