using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.DataAccess.Contract;

namespace DiscordBot.Framework.Contract.Modularity;

public abstract class CommandModuleBase : IGuildModule
{
    private static IDictionary<Type, Dictionary<string, MethodInfo>> _commandMethods;
    private readonly IModuleDataAccess _dataAccess;
    protected abstract Type RessourceType { get; }

    protected CommandModuleBase(IModuleDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
        _commandMethods = new Dictionary<Type, Dictionary<string, MethodInfo>>();
    }

    public abstract string ModuleUniqueIdentifier { get; }

    protected async Task<bool> IsEnabled(ulong id)
    {
        return await _dataAccess.IsModuleEnabledForGuild(id, ModuleUniqueIdentifier);
    }

    private string _language = string.Empty;

    public async Task InitializeAsync(SocketCommandContext context)
    {
        _language = await _dataAccess.GetUserLanguageAsync(context.User.Id);
    }

    public abstract Task<bool> CanExecuteAsync(ulong id, SocketCommandContext socketCommandContext);

    private void BuildCommandInfos()
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
        _commandMethods.Add(GetType(), moduleMethods);
    }

    public abstract Task ExecuteAsync(ICommandContext context);

    protected async Task ExecuteCommandsAsync(ICommandContext context)
    {
        if (!_commandMethods.ContainsKey(GetType()))
        {
            BuildCommandInfos();
        }

        var prefix = await _dataAccess.GetServerPrefixAsync(context.Guild.Id);
        var message = context.Message.Content;

        if (!message.StartsWith(prefix))
        {
            return;
        }

        var baseCommand = message.Remove(0, 1).Split(' ')[0].ToLower();
        var moduleMethods = _commandMethods[GetType()];
        if (!moduleMethods.ContainsKey(baseCommand))
        {
            return;
        }

        var method = moduleMethods[baseCommand];
        method.Invoke(this, new object[] { context });
        Console.WriteLine("Invoking " + method.Name);
    }

    protected async Task RequireArg(ICommandContext context, int count = 1, string errorMessage = null)
    {
        var args = context.Message.Content.Split(' ').Skip(1);
        if (args.Count() < count)
        {
            await context.Channel.SendMessageAsync(errorMessage ?? Localize(nameof(BaseRessources.Error_ArgumentCount), typeof(BaseRessources)));
            throw new ArgumentException(errorMessage ??
                                        $"{ModuleUniqueIdentifier}: Require Args: {count}, found {context.Message.Content}");
        }
    }

    protected async Task<int> RequireIntArg(ICommandContext context, int position = 1)
    {
        await RequireArg(context, position);
        var arg = context.Message.Content.Split(' ').Skip(position).First();
        if (int.TryParse(arg, out var result))
        {
            return result;
        }

        await context.Channel.SendMessageAsync(string.Format(Localize(nameof(BaseRessources.Error_NotAnInt), typeof(BaseRessources)), arg));
        throw new ArgumentException($"{ModuleUniqueIdentifier}: Required Int Arg: {arg}");
    }

    protected async Task<long> RequireLongArg(ICommandContext context, int position = 1)
    {
        await RequireArg(context, position);
        var arg = context.Message.Content.Split(' ').Skip(position).First();
        if (long.TryParse(arg, out var result))
        {
            return result;
        }

        await context.Channel.SendMessageAsync(string.Format(Localize(nameof(BaseRessources.Error_NotAnInt), typeof(BaseRessources)), arg));
        throw new ArgumentException($"{ModuleUniqueIdentifier}: Required Long Arg: {arg}");
    }

    protected async Task<ulong> RequireUlongAsync(ICommandContext context, int position = 1)
    {
        await RequireArg(context, position);
        var arg = context.Message.Content.Split(' ').Skip(position).First();
        if (ulong.TryParse(arg, out var result))
        {
            return result;
        }

        await context.Channel.SendMessageAsync(string.Format(Localize(nameof(BaseRessources.Error_NotAnInt), typeof(BaseRessources)), arg));
        throw new ArgumentException($"{ModuleUniqueIdentifier}: Required ULong Arg: {arg}");
    }


    protected async Task<string> RequireReminderArg(ICommandContext context, int position = 1)
    {
        await RequireArg(context, position);
        var args = context.Message.Content.Split(' ').Skip(position);
        var output = args.Aggregate(string.Empty, (current, arg) => current + $"{arg} ");
        return output.Trim();
    }

    protected async Task<string> RequireReminderOrEmpty(ICommandContext context, int position = 1)
    {
        var args = context.Message.Content.Split(' ').Skip(position);
        var output = args.Aggregate(string.Empty, (current, arg) => current + $"{arg} ");
        return await Task.FromResult(output.Trim());
    }

    protected int RequireIntArgOrDefault(ICommandContext context, int position = 1, int defaultValue = 0)
    {
        var args = context.Message.Content.Split(' ').Skip(position);
        var arg = args.FirstOrDefault();
        return int.TryParse(arg, out var result) ? result : defaultValue;
    }

    protected async Task<string> RequireString(ICommandContext context, int position = 1)
    {
        await RequireArg(context, position);
        return context.Message.Content.Split(' ').Skip(position).First();
    }

    protected async Task RequirePermissionAsync(ICommandContext context, GuildPermission permission)
    {
        var hasPermission = (await context.Guild.GetUserAsync(context.User.Id)).GuildPermissions.Has(permission);
        if (!hasPermission)
        {
            throw new InvalidOperationException(
                $"{ModuleUniqueIdentifier}: Keine Berechtigung in '{context.Guild.Name}'");
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