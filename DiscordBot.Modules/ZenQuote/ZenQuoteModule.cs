using Autofac;
using DiscordBot.Framework.Contract.Modularity;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.ZenQuote;

public class ZenQuoteModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<ZenQuoteTask>().As<ITimedAction>();
        builder.RegisterType<ZenQuoteCommands>().As<ICommandModule>();
    }
}