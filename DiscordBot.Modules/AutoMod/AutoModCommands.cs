using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.AutoMod.Violation;
using DiscordBot.Framework.Contract.Modularity;
using DiscordBot.Modules.AutoMod.KeyValueValidationStrategies;

namespace DiscordBot.Modules.AutoMod;

internal class AutoModCommands : CommandModuleBase, ICommandModule
{
    private readonly IEnumerable<IKeyValueValidationStrategy> _strategies;
    private readonly AutoModManager _manager;

    public AutoModCommands(IModuleDataAccess dataAccess, AutoModManager manager,
        IEnumerable<IKeyValueValidationStrategy> strategies) : base(dataAccess)
    {
        _strategies = strategies;
        _manager = manager;
    }

    protected override Type RessourceType => typeof(AutoModRessources);
    public override string ModuleUniqueIdentifier => "AUTOMOD";

    [Command("automod-listconfigs")]
    [Description("Lists all available configs for a module")]
    [Parameter(Name = "module", Description = "The module to list configs for", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.String)]
    public async Task ListConfigsAsync(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);
        await RequirePermissionAsync(context, guild, GuildPermission.Administrator);
        var module = await RequireString(context);
        var configs = _manager.GetAvailableConfigs(module);
        if (configs == null)
        {
            await context.RespondAsync(
                string.Format(Localize(nameof(AutoModRessources.Error_RuleDoesNotExist)), module));
            return;
        }

        var content = configs.Aggregate(string.Empty, (current, config) => current + $"{config.Key}: {config.Value}\n");
        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithDescription(content);
        embedBuilder.WithTitle(string.Format(Localize(nameof(AutoModRessources.Title_PossibleConfigs)), module));
        embedBuilder.WithColor(Color.DarkBlue);
        embedBuilder.WithCurrentTimestamp();
        await context.RespondAsync("", new[] { embedBuilder.Build() });
    }

    [Command("automod-list")]
    [Description("Lists all available automod modules")]
    public async Task ListModulesAsync(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);
        await RequirePermissionAsync(context, guild, GuildPermission.Administrator);
        var modules = _manager.GetModules(guild.Id);
        var content = string.Empty;
        foreach (var module in modules)
        {
            var activity = module.Value
                ? Localize(nameof(AutoModRessources.Identifier_Active))
                : Localize(nameof(AutoModRessources.Identifier_Inactive));
            content += $"{module.Key}: {activity}\n";
        }

        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithDescription(content);
        embedBuilder.WithTitle(Localize(nameof(AutoModRessources.Title_AvailableRules)));
        embedBuilder.WithColor(Color.DarkBlue);
        embedBuilder.WithCurrentTimestamp();
        await context.RespondAsync("", new[] { embedBuilder.Build() });
    }

    [Command("automod-config")]
    [Description("Changes a configuration value for a module")]
    [Parameter(Name = "module", Description = "The module to change the config for", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.String)]
    [Parameter(Name = "key", Description = "The value to be changed", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.String)]
    [Parameter(Name = "value", Description = "The value to be changed to", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.String)]
    public async Task ConfigureModuleAsync(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);
        var module = await RequireString(context);
        await RequireExistingRule(context, module);
        if (!_manager.IsRuleEnabledForGuild(module, guild.Id))
        {
            await context.RespondAsync(
                string.Format(Localize(nameof(AutoModRessources.Error_RuleNotActive)), module));
            return;
        }

        var key = await RequireString(context, 2);
        var value = (await RequireString(context, 3)).Trim();
        if (string.IsNullOrEmpty(value))
        {
            value = _manager.GetConfigValue(module, guild.Id, key);
            if (value == null)
            {
                await context.RespondAsync(
                    string.Format(Localize(nameof(AutoModRessources.Error_ConfigUnset)), key, module));
            }
            else
            {
                await context.RespondAsync((Localize(nameof(AutoModRessources.Message_SavedValue))));
            }

            return;
        }

        var type = _manager.GetValueTypeForRuleAndKey(module, key);
        await ValidateKeyValuePair(type, module, key, value, context);

        await _manager.SetValue(module, key, value, guild.Id);
        await context.RespondAsync(Localize(nameof(AutoModRessources.Message_SetValue)));
    }

    private async Task ValidateKeyValuePair(ConfigurationValueType type, string module, string key, string value,
        SocketSlashCommand context)
    {
        var strategy = _strategies.Single(strategy => strategy.IsResponsible(type));
        await strategy.ExecuteAsync(module, key, value, context);
    }

    [Command("automod-enable")]
    [Description("Enables a rule")]
    [Parameter(Name = "rule", Description = "The rule to enable", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.String)]
    public async Task EnableModuleAsync(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);
        await RequirePermissionAsync(context, guild, GuildPermission.Administrator);
        var module = await RequireString(context);
        await RequireExistingRule(context, module);
        if (_manager.IsRuleEnabledForGuild(module, guild.Id))
        {
            await context.RespondAsync(Localize(nameof(AutoModRessources.Error_RuleAlreadyActive)));
            return;
        }

        await _manager.EnableRuleAsync(module, guild.Id);
        await context.RespondAsync(Localize(nameof(AutoModRessources.Message_RuleActivated)));
    }

    [Command("automod-disable")]
    [Description("Disables a rule")]
    [Parameter(Name = "rule", Description = "The rule to disable", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.String)]
    public async Task DisableModuleAsync(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);
        await RequirePermissionAsync(context, guild, GuildPermission.Administrator);
        var module = await RequireString(context);
        await RequireExistingRule(context, module);
        if (!_manager.IsRuleEnabledForGuild(module, guild.Id))
        {
            await context.RespondAsync(Localize(nameof(AutoModRessources.Error_RuleNotActive)));
            return;
        }

        await _manager.DisableRuleAsync(module, guild.Id);
        await context.RespondAsync(Localize(nameof(AutoModRessources.Identifier_Inactive)));
    }

    private async Task RequireExistingRule(SocketSlashCommand context, string rule)
    {
        if (!_manager.ExistsRule(rule))
        {
            await context.RespondAsync(Localize(nameof(AutoModRessources.Error_RuleDoesNotExist)));
            throw new ArgumentException();
        }
    }
}