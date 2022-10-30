using Autofac;
using DiscordBot.DataAccess.Contract.UserConfiguration;
using DiscordBot.DataAccess.Modules.UserConfiguration.Domain;
using DiscordBot.DataAccess.Modules.UserConfiguration.Repository;

namespace DiscordBot.DataAccess.Modules.UserConfiguration;

internal class UserConfigurationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<UserConfigurationRepository>().As<IUserConfigurationRepository>();
        builder.RegisterType<UserConfigurationDomain>().As<IUserConfigurationDomain>();
        builder.RegisterType<UserConfigurationCache>().SingleInstance();
    }
}