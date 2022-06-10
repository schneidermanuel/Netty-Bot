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
    private IDictionary<string, MethodInfo> _commandMethods;
    private readonly IModuleDataAccess _dataAccess;
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

    public abstract Task<bool> CanExecuteAsync(ulong id, SocketCommandContext socketCommandContext);

    private void BuildCommandInfos()
    {
        _commandMethods = new Dictionary<string, MethodInfo>();
        var methods = GetType().GetMethods()
            .Where(m => m.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0).ToList();
        foreach (var methodInfo in methods)
        {
            var attribute = (CommandAttribute)methodInfo.GetCustomAttribute(typeof(CommandAttribute));
            Debug.Assert(attribute != null, nameof(attribute) + " != null");
            _commandMethods.Add(attribute.Text.ToLower(), methodInfo);
        }
    }

    public abstract Task ExecuteAsync(ICommandContext context);

    public async Task ExecuteCommandsAsync(ICommandContext context)
    {
        if (_commandMethods == null)
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
        if (!_commandMethods.ContainsKey(baseCommand))
        {
            return;
        }

        var method = _commandMethods[baseCommand];
        method.Invoke(this, new[] { context });
        Console.WriteLine("Invoking " + method.Name);
    }

    protected async Task RequireArg(ICommandContext context, int count = 1, string errorMessage = null)
    {
        var args = context.Message.Content.Split(' ').Skip(1);
        if (args.Count() < count)
        {
            await context.Channel.SendMessageAsync("Es wurden nicht genügende Argumente mitgegeben. ");
            throw new ArgumentException(errorMessage ??
                                        $"{ModuleUniqueIdentifier}: Require Args: {count}, found {context.Message.Content}");
        }
    }

    protected async Task<int> RequireIntArg(ICommandContext context, int position = 1)
    {
        await RequireArg(context, position);
        var args = context.Message.Content.Split(' ').Skip(position).First();
        if (int.TryParse(args, out var result))
        {
            return result;
        }

        await context.Channel.SendMessageAsync(
            $"Der Ausdruck '{args}' konnte nicht konvertiert werden. Erwartet: Zahl");
        throw new ArgumentException($"{ModuleUniqueIdentifier}: Required Int Arg: {args}");
    }

    protected async Task<long> RequireLongArg(ICommandContext context, int position = 1)
    {
        await RequireArg(context, position);
        var args = context.Message.Content.Split(' ').Skip(position).First();
        if (long.TryParse(args, out var result))
        {
            return result;
        }

        await context.Channel.SendMessageAsync(
            $"Der Ausdruck '{args}' konnte nicht konvertiert werden. Erwartet: Zahl");
        throw new ArgumentException($"{ModuleUniqueIdentifier}: Required Long Arg: {args}");
    }

    protected async Task<ulong> RequireUlongAsync(ICommandContext context, int position = 1)
    {
        await RequireArg(context, position);
        var args = context.Message.Content.Split(' ').Skip(position).First();
        if (ulong.TryParse(args, out var result))
        {
            return result;
        }

        await context.Channel.SendMessageAsync(
            $"Der Ausdruck '{args}' konnte nicht konvertiert werden. Erwartet: Zahl");
        throw new ArgumentException($"{ModuleUniqueIdentifier}: Required ULong Arg: {args}");
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
        return await Task.FromResult(output);
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

    protected string Localize(string ressource)
    {
        var cultureProperty = RessourceType.GetProperty("Culture", BindingFlags.NonPublic | BindingFlags.Static);
        cultureProperty?.SetValue(null, new CultureInfo("de"));
        return RessourceType.GetProperty(ressource, BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null)?.ToString();
    }
}