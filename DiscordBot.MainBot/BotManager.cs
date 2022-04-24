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
        var context = new SocketCommandContext(_client, arg as SocketUserMessage);
        if (context.User.IsBot)
        {
            return;
        }

        var botUser = context.Guild.GetUser(context.Client.Rest.CurrentUser.Id);
        if (!botUser.GuildPermissions.Administrator)
        {
            var prefix = await _dataAccess.GetServerPrefixAsync(context.Guild.Id);
            if (context.Message.Content.StartsWith(prefix))
            {
                await context.Channel.SendMessageAsync(
                    "BrainyXS ist leider zu Faul, die Berechtigungen des Bots zu überprüfen. Bitte geben Sie den Bot Administrator-Rechte, damit mein Code auch funktioniert. Danke");
            }

            return;
        }

        foreach (var module in _modules)
        {
            try
            {
                if (await module.CanExecuteAsync(context.Guild.Id, context))
                {
                    await module.ExecuteAsync(context);
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
        await _client.SetActivityAsync(new Game("Hustler-Lifestyle"));
        var postBootTasks = _timedActions.Where(x => x.GetExecutionTime() == ExecutionTime.PostBoot)
            .Select(x => x.Execute(_client));
        await Task.WhenAll(postBootTasks);
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
            await Task.Delay(new TimeSpan(1, 0, 0));
            var tasks = hourlyTasks.Select(x => x.Execute(_client));
            await Task.WhenAll(tasks);
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
            .Select(x => x.Execute(_client));
        await Task.WhenAll(dailyTasks);
    }
}