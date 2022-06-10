using Autofac;
using DiscordBot.DataAccess.Contract.UserConfiguration;
using DiscordBot.DataAccess.Modules.UserConfiguration.BusinessLogic;
using DiscordBot.DataAccess.Modules.UserConfiguration.Repository;

namespace DiscordBot.DataAccess.Modules.UserConfiguration;

internal class UserConfigurationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<UserConfigurationRepository>().As<IUserConfigurationRepository>();
        builder.RegisterType<UserConfigurationBusinessLogic>().As<IUserConfigurationBusinessLogic>();
        builder.RegisterType<UserConfigurationCache>().SingleInstance();
    }
}