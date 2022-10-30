using Autofac;
using DiscordBot.DataAccess.Contract.YoutubeNotification;
using DiscordBot.DataAccess.Modules.YoutubeNotifications.Domain;
using DiscordBot.DataAccess.Modules.YoutubeNotifications.Repository;

namespace DiscordBot.DataAccess.Modules.YoutubeNotifications;

internal class YoutubeNotificationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<YoutubeNotificationRepository>().As<IYoutubeNotificationRepository>();
        builder.RegisterType<YoutubeNotificationDomain>().As<IYoutubeNotificationDomain>();
    }
}