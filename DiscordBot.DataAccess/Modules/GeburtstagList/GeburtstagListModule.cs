using Autofac;
using DiscordBot.DataAccess.Contract.GeburtstagList;
using DiscordBot.DataAccess.Modules.GeburtstagList.BusinessLogic;
using DiscordBot.DataAccess.Modules.GeburtstagList.Repository;

namespace DiscordBot.DataAccess.Modules.GeburtstagList;

public class GeburtstagListModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<GeburtstagListBusinessLogic>().As<IGeburtstagListBusinessLogic>();
        builder.RegisterType<GeburtstagListRepository>().As<IGeburtstagListRepository>();
    }
}