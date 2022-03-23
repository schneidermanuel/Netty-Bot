using Autofac;
using DiscordBot.DataAccess.Contract.ReactionRoles;
using DiscordBot.DataAccess.Modules.ReactionRoles.BusinessLogic;
using DiscordBot.DataAccess.Modules.ReactionRoles.Repository;

namespace DiscordBot.DataAccess.Modules.ReactionRoles;

public class ReactionRolesDataAccessModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<ReactionRoleBusinessLogic>().As<IReactionRoleBusinessLogic>();
        builder.RegisterType<ReactionRolesRepository>().As<IReactionRolesRepository>();
    }
}