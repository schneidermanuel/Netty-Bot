using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.DataAccess.Contract;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.AutoMod;

internal class AutoModCommands : CommandModuleBase, IGuildModule
{
    private readonly AutoModManager _manager;

    public AutoModCommands(IModuleDataAccess dataAccess, AutoModManager manager) : base(dataAccess)
    {
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
        }
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