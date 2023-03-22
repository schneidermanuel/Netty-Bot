using System.Collections.Generic;
using System.Linq;
using DiscordBot.Framework.Contract.Interpretors.DateTime;

namespace DiscordBot.Framework.Interpretors.DateTime;

internal class DateTimeInterpretor : IDateTimeInterpretor
{
    private readonly IEnumerable<IDateTimeInterprationStrategy> _strategies;

    public DateTimeInterpretor(IEnumerable<IDateTimeInterprationStrategy> strategies)
    {
        _strategies = strategies;
    }

    public System.DateTime? Interpret(string data)
    {
        var relevantStrategy = _strategies
            .Where(strat =>
                strat.CanInterpret(data))
            .MaxBy(strat => strat.Priority);

        return relevantStrategy?.Interpret(data);
    }
}