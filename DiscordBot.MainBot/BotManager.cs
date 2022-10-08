using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;
using DiscordBot.Framework;
using DiscordBot.Framework.Contract;
using DiscordBot.Framework.Contract.Modularity;
using DiscordBot.Framework.Contract.TimedAction;

// ReSharper disable LocalizableElement

namespace DiscordBot.MainBot;

public class BotManager
{
    private readonly IEnumerable<IGuildModule> _modules;
    private readonly IEnumerable<ITimedAction> _timedActions;
    private readonly IModuleDataAccess _dataAccess;
    private readonly IEnumerable<ICommandModule> _commandModules;
    private bool _isReady;

    public static readonly DiscordSocketClient Client = new(new DiscordSocketConfig
    {
        GatewayIntents = GatewayIntents.All
    });

    private Dictionary<string, CommandInfos> _slashCommands;

    public BotManager(IEnumerable<IGuildModule> modules,
        IEnumerable<ITimedAction> timedActions,
        IModuleDataAccess dataAccess,
        IEnumerable<ICommandModule> commandModules)
    {
        _modules = modules;
        _timedActions = timedActions;
        _dataAccess = dataAccess;
        _commandModules = commandModules;
    }

    public async Task StartSystemAsync()
    {
        _isReady = false;
        _slashCommands = new Dictionary<string, CommandInfos>();
        foreach (var commandModule in _commandModules)
        {
            var commands = commandModule.BuildCommandInfos();
            foreach (var command in commands)
            {
                _slashCommands.Add(command.Key,
                    new CommandInfos { CommandModule = commandModule, MethodInfo = command.Value });
            }
        }

        Client.Ready += ClientReady;
        Client.Log += Log;
        Client.MessageReceived += MessageRecieved;
        Client.SlashCommandExecuted += SlashCommandRecieved;
        await Client.LoginAsync(TokenType.Bot, BotClientConstants.BotToken);
        Console.WriteLine("Logged in");
        await Client.StartAsync();
        Console.WriteLine("Startet System");
    }

    private async Task SlashCommandRecieved(SocketSlashCommand slashCommand)
    {
        if (slashCommand.Data.Name == "help")
        {
            await slashCommand.RespondAsync("check https://netty-bot.com/ for help");
            return;
        }

        try
        {
            var command = _slashCommands.Single(cmd => cmd.Key == slashCommand.CommandName);
            var commandInfo = command.Value;
            await commandInfo.CommandModule.InitializeAsync(slashCommand);
            commandInfo.MethodInfo.Invoke(commandInfo.CommandModule, new object[] { slashCommand });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private async Task MessageRecieved(SocketMessage arg)
    {
        if (!_isReady)
        {
            return;
        }

        var context = new SocketCommandContext(Client, arg as SocketUserMessage);
        if (context.User.IsBot)
        {
            return;
        }

        foreach (var module in _modules)
        {
            await module.InitializeAsync(context);
            try
            {
                if (await module.CanExecuteAsync(context.Guild.Id, context))
                {
                    _ = Task.Run(() => module.ExecuteAsync(context));
                }
            }
            catch (InvalidOperationException)
            {
                // Ignored for now
            }
            catch (ArgumentException)
            {
                // Ignored for now
            }
            catch (Exception e)
            {
                Console.WriteLine($"[{module.ModuleUniqueIdentifier}] {e.Message}");
            }
        }
    }

    private async Task Log(LogMessage arg)
    {
        Console.WriteLine(arg.Message);
        await Task.CompletedTask;
    }

    private async Task ClientReady()
    {
        Client.Ready -= ClientReady;
        await Client.SetStatusAsync(UserStatus.Online);
        await Client.SetActivityAsync(new Game("Booting..."));

        await RegisterSlashCommandsAsync();
        var builder = new SlashCommandBuilder();
        builder.WithName("help");
        builder.WithDescription("Sends some help");
        await Client.CreateGlobalApplicationCommandAsync(builder.Build());

        var postBootTasks = _timedActions.Where(x => x.GetExecutionTime() == ExecutionTime.PostBoot)
            .Select(x => x.ExecuteAsync(Client));
        await Task.WhenAll(postBootTasks);
        _isReady = true;
        var dailyStuffThread = new Thread(DailyStuff);
        var hourlyStuffThread = new Thread(HourlyStuff);
        dailyStuffThread.Start();
        hourlyStuffThread.Start();
    }

    private async void HourlyStuff()
    {
        var hourlyTasks = _timedActions.Where(x => x.GetExecutionTime() == ExecutionTime.Hourly).ToArray();
        while (true)
        {
            var tasks = hourlyTasks.Select(x => x.ExecuteAsync(Client));
            await Task.WhenAll(tasks);
            await Task.Delay(new TimeSpan(1, 0, 0));
        }
    }

    private async void DailyStuff()
    {
        if (!MainConfig.SkipDaily)
        {
            await DoDailyTasks();
        }

        while (true)
        {
            var now = DateTime.Now;
            var nextDate = now.Hour >= 5 ? now.AddDays(1) : now;
            var nextTime = new DateTime(nextDate.Year, nextDate.Month, nextDate.Day, 5, 0, 0);
            Console.WriteLine("Next time for Daily Shit: " + nextTime.ToString(CultureInfo.InvariantCulture));
            var duration = nextTime - now;
            await Task.Delay(duration);
            await DoDailyTasks();
        }
    }

    private async Task DoDailyTasks()
    {
        var dailyTasks = _timedActions.Where(x => x.GetExecutionTime() == ExecutionTime.Daily)
            .Select(x => x.ExecuteAsync(Client));
        await Task.WhenAll(dailyTasks);
    }

    private async Task RegisterSlashCommandsAsync()
    {
        try
        {
            foreach (var command in _slashCommands)
            {
                Console.WriteLine("registering " + command.Key);
                var builder = new SlashCommandBuilder();
                builder.WithName(command.Key.ToLower());
                var methodInfo = command.Value;
                var description = methodInfo.MethodInfo.GetCustomAttribute<DescriptionAttribute>()?.Description ??
                                  "missing description";
                builder.WithDescription(description);
                foreach (var parameterAttribute in methodInfo.MethodInfo.GetCustomAttributes<ParameterAttribute>())
                {
                    builder.AddOption(parameterAttribute.Name.ToLower(), parameterAttribute.ParameterType,
                        parameterAttribute.Description, !parameterAttribute.IsOptional);
                }

                try
                {
                    await Client.CreateGlobalApplicationCommandAsync(builder.Build());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}