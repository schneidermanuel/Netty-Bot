using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using DiscordBot.DataAccess;
using DiscordBot.Framework;
using DiscordBot.Framework.BootSteps;
using DiscordBot.Framework.Contract.Boot;
using DiscordBot.Modules;
using DiscordBot.MusicPlayer;
using DiscordBot.PubSub;
using DiscordBot.PubSub.Backend;
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

        if (args.Length > 1 && args[1] == "Debug")
        {
            MainConfig.Debug = true;
            Console.WriteLine("STARTING IN DEBUG MODE");
        }
        Configurator.Configure();
        BuildContainer();
        BootAsync().GetAwaiter().GetResult();
        var manager = _container.Resolve<BotManager>();
        manager.StartSystemAsync().GetAwaiter().GetResult();
        Task.Delay(-1).GetAwaiter().GetResult();
    }

    private static void BuildContainer()
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule<DiscordBotModulesModule>();
        builder.RegisterModule<DataAccessModule>();
        builder.RegisterType<BotManager>();
        builder.RegisterModule<MusicPlayerModule>();
        builder.RegisterModule<VictoriaModule>();
        builder.RegisterModule<PubSubModule>();
        builder.RegisterModule<DiscordBotPubSubBackendModule>();
        builder.RegisterModule<FrameworkModule>();
        builder.RegisterInstance(BotManager.Client);
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