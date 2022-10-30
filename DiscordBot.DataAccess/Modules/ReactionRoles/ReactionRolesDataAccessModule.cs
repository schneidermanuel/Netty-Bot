using Autofac;
using DiscordBot.DataAccess.Contract.ReactionRoles;
using DiscordBot.DataAccess.Modules.ReactionRoles.Domain;
using DiscordBot.DataAccess.Modules.ReactionRoles.Repository;

namespace DiscordBot.DataAccess.Modules.ReactionRoles;

public class ReactionRolesDataAccessModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<ReactionRoleDomain>().As<IReactionRoleDomain>();
        builder.RegisterType<ReactionRolesRepository>().As<IReactionRolesRepository>();
    }
}