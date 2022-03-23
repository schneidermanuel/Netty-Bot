using Autofac;
using DiscordBot.DataAccess.Contract.AutoRole;
using DiscordBot.DataAccess.Modules.AutoRole.BusinessLogic;
using DiscordBot.DataAccess.Modules.AutoRole.Repository;

namespace DiscordBot.DataAccess.Modules.AutoRole;

public class AutoRoleModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<AutoRoleBusinessLogic>().As<IAutoRoleBusinessLogic>();
        builder.RegisterType<AutoRoleRepository>().As<IAutoRoleRepository>();
    }
}