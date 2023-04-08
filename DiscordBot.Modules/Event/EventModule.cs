using Autofac;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.Event;

internal class EventModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<EventCommands>().As<ICommandModule>();
        builder.RegisterType<EventLifesycleButtonListener>().As<IButtonListener>();
        builder.RegisterType<EventModalListener>().As<IModalListener>();
    }
}