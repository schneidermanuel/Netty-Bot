using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;

namespace DiscordBot.Framework.Contract.Modularity;

public abstract class CommandModuleBase : ICommandModule
{
    protected abstract Type RessourceType { get; }

    protected CommandModuleBase(IModuleDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    public abstract string ModuleUniqueIdentifier { get; }

    protected async Task<bool> IsEnabled(ulong id)
    {
        return await _dataAccess.IsModuleEnabledForGuild(id, ModuleUniqueIdentifier);
    }

    private string _language = string.Empty;
    private readonly IModuleDataAccess _dataAccess;

    public async Task InitializeAsync(SocketSlashCommand context)
    {
        _language = await _dataAccess.GetUserLanguageAsync(context.User.Id);
    }

    protected async Task<IGuild> RequireGuild(SocketSlashCommand context)
    {
        if (context.Channel is IGuildChannel guildChannel)
        {
            return guildChannel.Guild;
        }

        await context.RespondAsync(Localize(nameof(BaseRessources.Error_NotAGuild), typeof(BaseRessources)));
        throw new InvalidOperationException();
    }

    public Dictionary<string, MethodInfo> BuildCommandInfos()
    {
        var methods = GetType().GetMethods()
            .Where(m => m.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0).ToList();
        var moduleMethods = new Dictionary<string, MethodInfo>();
        foreach (var methodInfo in methods)
        {
            var attribute = (CommandAttribute)methodInfo.GetCustomAttribute(typeof(CommandAttribute));
            Debug.Assert(attribute != null, nameof(attribute) + " != null");
            moduleMethods.Add(attribute.Text.ToLower(), methodInfo);
        }

        return moduleMethods;
    }

    public virtual async Task ExecuteAsync(ICommandContext context)
    {
        await Task.CompletedTask;
    }

    protected async Task RequireArg(SocketSlashCommand context, int count = 1, string errorMessage = null)
    {
        var args = context.Data.Options;
        if (args.Count() < count)
        {
            await context.RespondAsync(errorMessage ??
                                       Localize(nameof(BaseRessources.Error_ArgumentCount),
                                           typeof(BaseRessources)));
            throw new ArgumentException(errorMessage ??
                                        $"{ModuleUniqueIdentifier}: Require Args: {count}, found {context.Data.Options.Count() - 1}");
        }
    }

    protected async Task<int> RequireIntArg(SocketSlashCommand context, int position = 1)
    {
        await RequireArg(context, position);
        var arg = context.Data.Options.Skip(position - 1).First().Value.ToString();
        if (int.TryParse(arg, out var result))
        {
            return result;
        }

        await context.RespondAsync(
            string.Format(Localize(nameof(BaseRessources.Error_NotAnInt), typeof(BaseRessources)), arg));
        throw new ArgumentException($"{ModuleUniqueIdentifier}: Required Int Arg: {arg}");
    }

    protected async Task<long> RequireLongArg(SocketSlashCommand context, int position = 1)
    {
        await RequireArg(context, position);
        var arg = context.Data.Options.Skip(position - 1).First().Value.ToString();
        if (long.TryParse(arg, out var result))
        {
            return result;
        }

        await context.RespondAsync(
            string.Format(Localize(nameof(BaseRessources.Error_NotAnInt), typeof(BaseRessources)), arg));
        throw new ArgumentException($"{ModuleUniqueIdentifier}: Required Long Arg: {arg}");
    }

    protected async Task<ulong> RequireUlongAsync(SocketSlashCommand context, int position = 1)
    {
        await RequireArg(context, position);
        var arg = context.Data.Options.Skip(position - 1).First().Value.ToString();
        if (ulong.TryParse(arg, out var result))
        {
            return result;
        }

        await context.RespondAsync(
            string.Format(Localize(nameof(BaseRessources.Error_NotAnInt), typeof(BaseRessources)), arg));
        throw new ArgumentException($"{ModuleUniqueIdentifier}: Required ULong Arg: {arg}");
    }

    protected int RequireIntArgOrDefault(SocketSlashCommand context, int position = 1, int defaultValue = 0)
    {
        var args = context.Data.Options.Skip(position - 1);
        var arg = args.FirstOrDefault()?.Value.ToString() ?? string.Empty;
        return int.TryParse(arg, out var result) ? result : defaultValue;
    }

    protected async Task<string> RequireString(SocketSlashCommand context, int position = 1)
    {
        await RequireArg(context, position);
        return context.Data.Options.Skip(position - 1).First().Value.ToString();
    }
    protected string RequireStringOrEmpty(SocketSlashCommand context, int position = 1)
    {
        var arg = context.Data.Options.Skip(position - 1).FirstOrDefault()?.Value ?? string.Empty;
        return arg.ToString();
    }


    protected async Task<IRole> RequireRoleAsync(SocketSlashCommand context, int position = 1)
    {
        await RequireArg(context, position);
        var arg = context.Data.Options.Skip(position - 1).First().Value;
        if (arg is IRole role)
        {
            return role;
        }

        return null;
    }

    protected async Task RequirePermissionAsync(SocketSlashCommand context, IGuild guild, GuildPermission permission)
    {
        var hasPermission = (await guild.GetUserAsync(context.User.Id)).GuildPermissions.Has(permission);
        if (!hasPermission)
        {
            throw new InvalidOperationException(
                $"{ModuleUniqueIdentifier}: Keine Berechtigung in '{guild.Name}'");
        }
    }

    protected string Localize(string ressource, Type ressourceType = null)
    {
        ressourceType ??= RessourceType;
        var language = _language;
        lock (ressourceType)
        {
            var cultureProperty = ressourceType.GetProperty("Culture", BindingFlags.NonPublic | BindingFlags.Static);
            cultureProperty?.SetValue(null, new CultureInfo(language));
            return ressourceType.GetProperty(ressource, BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null)
                ?.ToString();
        }
    }
}