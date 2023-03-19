using Autofac;
using DiscordBot.DataAccess.Contract.GuildConfiguration;
using DiscordBot.DataAccess.Modules.GuildConfig.Domain;
using DiscordBot.DataAccess.Modules.GuildConfig.Repository;

namespace DiscordBot.DataAccess.Modules.GuildConfig;

internal class GuildConfigModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<GuildConfigCache>().SingleInstance();
        builder.RegisterType<GuildConfigDomain>().As<IGuildConfigDomain>();
        builder.RegisterType<GuildConfigRepository>().As<IGuildConfigRepository>();
    }
}