using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.AutoMod.Violation;
using DiscordBot.Framework.Contract.Modularity;
using DiscordBot.Modules.AutoMod.KeyValueValidationStrategies;
using DiscordBot.Modules.AutoMod.Rules;

namespace DiscordBot.Modules.AutoMod;

internal class AutoModCommands : CommandModuleBase, IGuildModule
{
    private readonly IEnumerable<IKeyValueValidationStrategy> _strategies;
    private readonly AutoModManager _manager;

    public AutoModCommands(IModuleDataAccess dataAccess, AutoModManager manager,
        IEnumerable<IKeyValueValidationStrategy> strategies) : base(dataAccess)
    {
        _strategies = strategies;
        _manager = manager;
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

    public override string ModuleUniqueIdentifier => "AUTOMOD";

    [Command("autoMod")]
    public async Task AutoModCommand(ICommandContext context)
    {
        var action = await RequireString(context);
        switch (action)
        {
            case "enable":
                await EnableModuleAsync(context);
                break;
            case "disable":
                await DisableModuleAsync(context);
                break;
            case "config":
                await ConfigureModuleAsync(context);
                break;
            case "list":
                await ListModulesAsync(context);
                break;
            case "list-configs":
                await ListConfigsAsync(context);
                break;
        }
    }

    private async Task ListConfigsAsync(ICommandContext context)
    {
        var module = await RequireString(context, 2);
        var configs = _manager.GetAvailableConfigs(module);
        if (configs == null)
        {
            await context.Channel.SendMessageAsync($"Die Regel {module} existiert nicht");
            return;
        }

        var content = configs.Aggregate(string.Empty, (current, config) => current + $"{config.Key}: {config.Value}\n");
        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithDescription(content);
        embedBuilder.WithTitle($"Verfügbare AutoMod-Einstellungen für {module}");
        embedBuilder.WithColor(Color.DarkBlue);
        embedBuilder.WithCurrentTimestamp();
        await context.Channel.SendMessageAsync("", false, embedBuilder.Build());

    }

    private async Task ListModulesAsync(ICommandContext context)
    {
        var modules = _manager.GetModules(context.Guild.Id);
        var content = string.Empty;
        foreach (var module in modules)
        {
            var activity = module.Value ? "Aktiv" : "Inaktiv";
            content += $"{module.Key}: {activity}\n";
        }

        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithDescription(content);
        embedBuilder.WithTitle("Verfügbare AutoMod-Einstellungen");
        embedBuilder.WithColor(Color.DarkBlue);
        embedBuilder.WithCurrentTimestamp();
        await context.Channel.SendMessageAsync("", false, embedBuilder.Build());
    }

    private async Task ConfigureModuleAsync(ICommandContext context)
    {
        var module = await RequireString(context, 2);
        await RequireExistingRule(context, module);
        if (!_manager.IsRuleEnabledForGuild(module, context.Guild.Id))
        {
            await context.Channel.SendMessageAsync($"Die Regel '{module}' ist nicht aktiv");
            return;
        }

        var key = await RequireString(context, 3);
        var value = await RequireReminderArg(context, 4);

        var type = _manager.GetValueTypeForRuleAndKey(module, key);
        await ValidateKeyValuePair(type, module, key, value, context);

        await _manager.SetValue(module, key, value, context.Guild.Id);
        await context.Channel.SendMessageAsync("Der Wert wurde gespeichert!");
    }

    private async Task ValidateKeyValuePair(ConfigurationValueType type, string module, string key, string value,
        ICommandContext context)
    {
        var strategy = _strategies.Single(strategy => strategy.IsResponsible(type));
        await strategy.ExecuteAsync(module, key, value, context);
    }

    private async Task EnableModuleAsync(ICommandContext context)
    {
        var module = await RequireString(context, 2);
        await RequireExistingRule(context, module);
        if (_manager.IsRuleEnabledForGuild(module, context.Guild.Id))
        {
            await context.Channel.SendMessageAsync("Diese Regel ist bereits aktiv");
            return;
        }

        await _manager.EnableRuleAsync(module, context.Guild.Id);
        await context.Channel.SendMessageAsync($"Die Regel '{module}' wurde aktiviert");
    }

    private async Task DisableModuleAsync(ICommandContext context)
    {
        var module = await RequireString(context, 2);
        await RequireExistingRule(context, module);
        if (!_manager.IsRuleEnabledForGuild(module, context.Guild.Id))
        {
            await context.Channel.SendMessageAsync("Diese Regel ist bereits inaktiv");
            return;
        }

        await _manager.DisableRuleAsync(module, context.Guild.Id);
        await context.Channel.SendMessageAsync($"Die Regel '{module}' wurde deaktiviert");
    }

    private async Task RequireExistingRule(ICommandContext context, string rule)
    {
        if (!_manager.ExistsRule(rule))
        {
            await context.Channel.SendMessageAsync("Diese Regel existiert nicht");
            throw new ArgumentException();
        }
    }
}