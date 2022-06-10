using Autofac;
using DiscordBot.DataAccess.Contract.GuildConfiguration;
using DiscordBot.DataAccess.Modules.GuildConfig.BusinessLogic;
using DiscordBot.DataAccess.Modules.GuildConfig.Repository;

namespace DiscordBot.DataAccess.Modules.GuildConfig;

internal class GuildConfigModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<GuildConfigCache>().SingleInstance();
        builder.RegisterType<GuildConfigBusinessLogic>().As<IGuildConfigBusinessLogic>();
        builder.RegisterType<GuildConfigRepository>().As<IGuildConfigRepository>();
    }
}