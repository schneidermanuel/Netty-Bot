using Autofac;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Modules.AutoRole;
using DiscordBot.DataAccess.Modules.GeburtstagList;
using DiscordBot.DataAccess.Modules.MusicPlayer;
using DiscordBot.DataAccess.Modules.ReactionRoles;
using DiscordBot.DataAccess.Modules.TwitchNotifications;
using DiscordBot.DataAccess.Modules.ZenQuote;
using DiscordBot.DataAccess.NHibernate;

namespace DiscordBot.DataAccess;

public class DataAccessModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<ModuleDataAccess>().As<IModuleDataAccess>();
        builder.RegisterType<SessionFactoryProvider>().As<ISessionFactoryProvider>();
        builder.RegisterModule<ReactionRolesDataAccessModule>();
        builder.RegisterModule<ZenQuoteDataAccessModule>();
        builder.RegisterModule<GeburtstagListModule>();
        builder.RegisterModule<MusicPlayerModule>();
        builder.RegisterModule<AutoRoleModule>();
        builder.RegisterModule<TwitchNotificationsModule>();
    }
}