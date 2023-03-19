using Autofac;
using DiscordBot.DataAccess.Contract.GeburtstagList;
using DiscordBot.DataAccess.Modules.GeburtstagList.Domain;
using DiscordBot.DataAccess.Modules.GeburtstagList.Repository;

namespace DiscordBot.DataAccess.Modules.GeburtstagList;

public class GeburtstagListModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<GeburtstagListDomain>().As<IGeburtstagListDomain>();
        builder.RegisterType<GeburtstagListRepository>().As<IGeburtstagListRepository>();
    }
}