using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.AutoMod;

internal class AutoModChecker : IGuildModule
{
    private readonly IModuleDataAccess _dataAccess;
    private readonly AutoModManager _manager;

    public AutoModChecker(IModuleDataAccess dataAccess, AutoModManager manager)
    {
        _dataAccess = dataAccess;
        _manager = manager;
    }

    public async Task<bool> CanExecuteAsync(ulong id, SocketCommandContext socketCommandContext)
    {
        return await _dataAccess.IsModuleEnabledForGuild(id, ModuleUniqueIdentifier);
    }

    public async Task ExecuteAsync(ICommandContext context)
    {
        if (context.User is SocketGuildUser { GuildPermissions.Administrator: false })
        {
            var violation = await _manager.ProcessMessage(context);
            var reason = string.Format(await LocalizeAsync(violation.Reason, context.User.Id), context.User.Mention);
            await violation.Execute(context, reason);
        }
    }

    public string ModuleUniqueIdentifier => "AUTOMOD_EXECUTOR";

    public async Task InitializeAsync(SocketCommandContext context)
    {
        await Task.CompletedTask;
    }

    private async Task<string> LocalizeAsync(string ressource, ulong userId)
    {
        if (string.IsNullOrEmpty(ressource))
        {
            return string.Empty;
        }

        var language = await _dataAccess.GetUserLanguageAsync(userId);
        lock (typeof(AutoModRessources))
        {
            var cultureProperty =
                typeof(AutoModRessources).GetProperty("Culture", BindingFlags.NonPublic | BindingFlags.Static);
            cultureProperty?.SetValue(null, new CultureInfo(language));
            return typeof(AutoModRessources).GetProperty(ressource, BindingFlags.NonPublic | BindingFlags.Static)
                ?.GetValue(null)
                ?.ToString();
        }
    }
}