using Autofac;
using DiscordBot.DataAccess.Contract.MkCalculator;
using DiscordBot.DataAccess.Modules.MkCalculator.Domain;
using DiscordBot.DataAccess.Modules.MkCalculator.Repository;

namespace DiscordBot.DataAccess.Modules.MkCalculator;

internal class MkCalculatorModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<MkGameDomain>().As<IMkGameDomain>();
        builder.RegisterType<MkGameRepository>().As<IMkGameRepository>();
    }
}