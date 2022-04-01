using Autofac;
using DiscordBot.DataAccess.Contract.TwitchNotifications;
using DiscordBot.DataAccess.Modules.TwitchNotifications.BusinessLogic;
using DiscordBot.DataAccess.Modules.TwitchNotifications.Repository;

namespace DiscordBot.DataAccess.Modules.TwitchNotifications;

internal class TwitchNotificationsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<TwitchNotificationsBusinessLogic>().As<ITwitchNotificationsBusinessLogic>();
        builder.RegisterType<TwitchNotificationsRepository>().As<ITwitchNotificationsRepository>();
    }
}