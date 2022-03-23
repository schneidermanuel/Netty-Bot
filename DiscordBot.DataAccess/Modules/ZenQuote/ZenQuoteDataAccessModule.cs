using Autofac;
using DiscordBot.DataAccess.Contract.ZenQuote;
using DiscordBot.DataAccess.Modules.ZenQuote.BusinessLogic;
using DiscordBot.DataAccess.Modules.ZenQuote.Repository;

namespace DiscordBot.DataAccess.Modules.ZenQuote;

public class ZenQuoteDataAccessModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<ZenQuoteBusinessLogic>().As<IZenQuoteBusinessLogic>();
        builder.RegisterType<ZenQuoteRepository>().As<IZenQuoteRepository>();
    }
}