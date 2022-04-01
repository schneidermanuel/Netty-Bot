using Autofac;
using DiscordBot.Framework.Contract.Boot;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.TwitchNotifications;

internal class TwitchNotificationsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<TwitchNotificationsBootStep>().As<IBootStep>();
        builder.RegisterType<TwitchNotificationCommands>().As<IGuildModule>();
        builder.RegisterType<TwitchNotificationsManager>().AsSelf().SingleInstance();
    }
}