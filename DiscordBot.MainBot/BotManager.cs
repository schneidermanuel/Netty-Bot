using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

namespace DiscordBot.MainBot;

public class BotManager
{
    private const ulong ClientId = 898251551183896586;
    private readonly IEnumerable<IGuildModule> _modules;
    private readonly IEnumerable<ITimedAction> _timedActions;
    private readonly IModuleDataAccess _dataAccess;
    private bool _isReady;

    public static DiscordSocketClient _client = new DiscordSocketClient(new DiscordSocketConfig
    {
        GatewayIntents = GatewayIntents.All
    });

    public BotManager(IEnumerable<IGuildModule> modules, IEnumerable<ITimedAction> timedActions,
        IModuleDataAccess dataAccess)
    {
        _modules = modules;
        _timedActions = timedActions;
        _dataAccess = dataAccess;
    }

    public async Task StartSystemAsync()
    {
        _isReady = false;
        _client.Ready += ClientReady;
        _client.Log += Log;
        _client.MessageReceived += MessageRecieved;
        await _client.LoginAsync(TokenType.Bot, BotClientConstants.BotToken);
        Console.WriteLine("Logged in");
        await _client.StartAsync();
        Console.WriteLine("Startet System");
    }

    private async Task MessageRecieved(SocketMessage arg)
    {
        if (!_isReady)
        {
            return;
        }
        var context = new SocketCommandContext(_client, arg as SocketUserMessage);
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
        _client.Ready -= ClientReady;
        await _client.SetStatusAsync(UserStatus.Online);
        await _client.SetActivityAsync(new Game("Booting..."));
        var postBootTasks = _timedActions.Where(x => x.GetExecutionTime() == ExecutionTime.PostBoot)
            .Select(x => x.ExecuteAsync(_client));
        await Task.WhenAll(postBootTasks);
        _isReady = true;
        var dailyStuffThread = new Thread(DailyStuff);
        var hourlyStuffThread = new Thread(HourlyStuff);
        dailyStuffThread.Start();
        hourlyStuffThread.Start();
        foreach (var clientGuild in _client.Guilds)
        {
            try
            {
                var commands = await clientGuild.GetApplicationCommandsAsync();
                // Everything Good, bot has Command Access
            }
            catch (Exception e)
            {
                // Not Good, Bot does NOT have Command Scope Access
                await clientGuild.Owner.SendMessageAsync($"Hey, thanks for using Netty!\nWe have great news for you!\nFrom the 1. September, we will follow discord's new standard and switch to slash commands. " +
                                                         $"\nThis makes it way easier for you and your users to use our commands\n" +
                                                         $"However, I currentiy don't have the required permission on your Server '{clientGuild.Name}'.\n" +
                                                         $"In order to use the bot in the future, you need to give me the permission 'applications.commands'.\n" +
                                                         $"Thats an easy process! Just select my profile, click 'view profile' and press the blue 'Add to Server' button, and re-add the bot to your server.\n" +
                                                         $"No need to kick me out first! All your commands will be preserved.\n" +
                                                         $"If you have any questions, feel free to contant Brainy | Manuel#5332 for help.\n " +
                                                         $"There is a demonstration Video available at https://netty-bot.com/Slashcommands.mp4");
                Console.WriteLine($"Sent Message to '{clientGuild.Owner}' for '{clientGuild.Name}'");
            }
        }
    }

    private async void HourlyStuff()
    {
        var hourlyTasks = _timedActions.Where(x => x.GetExecutionTime() == ExecutionTime.Hourly).ToArray();
        while (true)
        {
            var tasks = hourlyTasks.Select(x => x.ExecuteAsync(_client));
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
            .Select(x => x.ExecuteAsync(_client));
        await Task.WhenAll(dailyTasks);
    }
}