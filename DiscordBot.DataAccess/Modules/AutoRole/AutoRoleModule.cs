using Autofac;
using DiscordBot.DataAccess.Contract.AutoRole;
using DiscordBot.DataAccess.Modules.AutoRole.Domain;
using DiscordBot.DataAccess.Modules.AutoRole.Repository;

namespace DiscordBot.DataAccess.Modules.AutoRole;

public class AutoRoleModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<AutoRoleDomain>().As<IAutoRoleDomain>();
        builder.RegisterType<AutoRoleRepository>().As<IAutoRoleRepository>();
    }
}