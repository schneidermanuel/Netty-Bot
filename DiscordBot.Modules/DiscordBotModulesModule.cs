using Autofac;
using DiscordBot.Modules.AutoMod;
using DiscordBot.Modules.AutoRole;
using DiscordBot.Modules.BirthdayList;
using DiscordBot.Modules.Configuration;
using DiscordBot.Modules.Event;
using DiscordBot.Modules.MarioKart;
using DiscordBot.Modules.Moderation;
using DiscordBot.Modules.MusicPlayer;
using DiscordBot.Modules.ReactionRoles;
using DiscordBot.Modules.ServerCounter;
using DiscordBot.Modules.TwitchNotifications;
using DiscordBot.Modules.TwitterNotification;
using DiscordBot.Modules.WebBackend;
using DiscordBot.Modules.YoutubeNotifications;
using DiscordBot.Modules.ZenQuote;

namespace DiscordBot.Modules;

public class DiscordBotModulesModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterModule<ReactionRolesModule>();
        builder.RegisterModule<ZenQuoteModule>();
        builder.RegisterModule<BirthdayListModule>();
        builder.RegisterModule<MusicPlayerModule>();
        builder.RegisterModule<TwitchNotificationsModule>();
        builder.RegisterModule<YoutubeNotificationsModule>();
        builder.RegisterModule<AutoRoleModule>();
        builder.RegisterModule<ModerationModule>();
        builder.RegisterModule<AutoModModule>();
        builder.RegisterModule<MkCalculatorModule>();
        builder.RegisterModule<ServerCoutnerModule>();
        builder.RegisterModule<ConfigurationModule>();
        builder.RegisterModule<EventModule>();
        builder.RegisterModule<TwitterModule>();
        builder.RegisterModule<WebBackendModule>();
    }
}