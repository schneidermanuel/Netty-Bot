using Autofac;
using DiscordBot.Framework.Contract.Boot;
using DiscordBot.Framework.Contract.Modularity;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.TwitchNotifications;

internal class TwitchNotificationsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<TwitchNotificationsBootStep>().As<IBootStep>();
        builder.RegisterType<TwitchNotificationCommands>().As<IGuildModule>();
        builder.RegisterType<TwitchNotificationReconnectStep>().As<ITimedAction>();
        builder.RegisterType<TwitchNotificationsManager>().AsSelf().SingleInstance();
    }
}