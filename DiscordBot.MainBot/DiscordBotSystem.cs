using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using DiscordBot.DataAccess;
using DiscordBot.Framework;
using DiscordBot.Framework.Contract.Boot;
using DiscordBot.Modules;
using DiscordBot.MusicPlayer;
using Victoria;

namespace DiscordBot.MainBot;

public class DiscordBotSystem
{
    private static IContainer _container;

    public static void Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "SkipDaily")
        {
            MainConfig.SkipDaily = true;
            Console.WriteLine("Skip Daily");
        }

        BuildContainer();
        BootAsync().GetAwaiter().GetResult();
        var manager = _container.Resolve<BotManager>();
        manager.StartSystemAsync().GetAwaiter().GetResult();
        Task.Delay(-1).GetAwaiter().GetResult();
    }

    private static void BuildContainer()
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule<FrameworkModule>();
        builder.RegisterModule<DiscordBotModulesModule>();
        builder.RegisterModule<DataAccessModule>();
        builder.RegisterType<BotManager>();
        builder.RegisterModule<MusicPlayerModule>();
        builder.RegisterModule<VictoriaModule>();
        builder.RegisterInstance(BotManager._client);
        _container = builder.Build();
    }

    private static async Task BootAsync()
    {
        var bootSteps = _container.Resolve<IEnumerable<IBootStep>>().ToList();
        var firstSteps = bootSteps.Where(step => step.StepPosition == BootOrder.First);
        var asyncSteps = bootSteps.Where(step => step.StepPosition == BootOrder.Async).ToList();
        var endSteps = bootSteps.Where(steps => steps.StepPosition == BootOrder.End).ToList();
        foreach (var bootStep in firstSteps)
        {
            await bootStep.BootAsync();
        }

        var asyncStepTasks = asyncSteps.Select(step => step.BootAsync());
        await Task.WhenAll(asyncStepTasks);
        var endStepTask = endSteps.Select(step => step.BootAsync());
        await Task.WhenAll(endStepTask);
    }
}