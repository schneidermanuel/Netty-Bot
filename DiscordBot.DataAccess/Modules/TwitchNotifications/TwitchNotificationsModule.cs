using Autofac;
using DiscordBot.DataAccess.Contract.TwitchNotifications;
using DiscordBot.DataAccess.Modules.TwitchNotifications.Domain;
using DiscordBot.DataAccess.Modules.TwitchNotifications.Repository;

namespace DiscordBot.DataAccess.Modules.TwitchNotifications;

internal class TwitchNotificationsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<TwitchNotificationsDomain>().As<ITwitchNotificationsDomain>();
        builder.RegisterType<TwitchNotificationsRepository>().As<ITwitchNotificationsRepository>();
    }
}