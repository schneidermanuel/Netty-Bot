using Autofac;
using DiscordBot.DataAccess.Contract.AutoMod;
using DiscordBot.DataAccess.Modules.AutoMod.BusinessLogic;
using DiscordBot.DataAccess.Modules.AutoMod.Repository;

namespace DiscordBot.DataAccess.Modules.AutoMod;

internal class AutoModModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<AutoModRepository>().As<IAutoModRepository>();
        builder.RegisterType<AutoModBusinessLogic>().As<IAutoModBusinessLogic>();
    }
}