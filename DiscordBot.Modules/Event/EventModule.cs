using Autofac;
using DiscordBot.Framework.Contract.Boot;
using DiscordBot.Framework.Contract.Modularity;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.Event;

internal class EventModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<EventCommands>().As<ICommandModule>();
        builder.RegisterType<EventLifesycleManager>().SingleInstance().AsSelf().As<IButtonListener>();
    }
}