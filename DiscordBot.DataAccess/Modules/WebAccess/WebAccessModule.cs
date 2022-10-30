using Autofac;
using DiscordBot.DataAccess.Modules.WebAccess.Domain;
using DiscordBot.DataAccess.Modules.WebAccess.Repository;

namespace DiscordBot.DataAccess.Modules.WebAccess;

internal class WebAccessModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<WebAccessRepository>().As<IWebAccessRepository>();
        builder.RegisterType<WebAccessDomain>().As<IWebAccessDomain>();
    }
}