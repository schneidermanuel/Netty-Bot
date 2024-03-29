﻿using Autofac;
using DiscordBot.Framework.Contract.Boot;
using DiscordBot.Framework.Contract.Modularity;
using DiscordBot.Framework.Contract.Modules.TwitchRegistrations;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.TwitchNotifications;

internal class TwitchNotificationsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<TwitchNotificationsBootStep>().As<IBootStep>();
        builder.RegisterType<TwitchNotificationCommands>().As<ICommandModule>();
        builder.RegisterType<TwitchNotificationsManager>().AsSelf().As<ITimedAction>().SingleInstance();
        builder.RegisterType<TwitchRefresher>().As<ITwitchRefresher>();
    }
}