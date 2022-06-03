using Autofac;
using DiscordBot.DataAccess.Contract.MkCalculator;
using DiscordBot.DataAccess.Modules.MkCalculator.BusinessLogic;
using DiscordBot.DataAccess.Modules.MkCalculator.Repository;

namespace DiscordBot.DataAccess.Modules.MkCalculator;

internal class MkCalculatorModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<MkGameBusinessLogic>().As<IMkGameBusinessLogic>();
        builder.RegisterType<MkGameRepository>().As<IMkGameRepository>();
    }
}