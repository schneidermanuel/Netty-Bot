using Autofac;
using DiscordBot.Framework.Contract.Interpretors.DateTime;

namespace DiscordBot.Framework.Interpretors.DateTime;

internal class DateTimeInterpretorModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<DateTimeFullHourInterprationStrategy>().As<IDateTimeInterprationStrategy>();
        builder.RegisterType<DateTimeHourMinuteInterprationStrategy>().As<IDateTimeInterprationStrategy>();
        builder.RegisterType<DateTimeParseInterprationStrategy>().As<IDateTimeInterprationStrategy>();
        builder.RegisterType<DateTimeInterpretor>().As<IDateTimeInterpretor>();
    }
}