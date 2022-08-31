using Autofac;
using DiscordBot.Framework.Contract.Boot;
using DiscordBot.Framework.Contract.Modularity;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.YoutubeNotifications;

internal class YoutubeNotificationsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<YoutubeNotificationsBootStep>().As<IBootStep>();
        builder.RegisterType<YoutubeNotificationCommands>().As<ICommandModule>();
        builder.RegisterType<RefreshYoutubeAction>().As<ITimedAction>();
        builder.RegisterType<YoutubeNotificationManager>().AsSelf().SingleInstance();
    }
}