namespace DiscordBot.Framework.Interpretors.DateTime;

internal class DateTimeFullHourInterprationStrategy : IDateTimeInterprationStrategy
{
    public bool CanInterpret(string data)
    {
        if (int.TryParse(data, out var hour))
        {
            return hour is >= 0 and < 24;
        }

        return false;
    }

    public int Priority => 10;

    public System.DateTime Interpret(string data)
    {
        var input = int.Parse(data);
        var now = System.DateTime.Now;

        var date = new System.DateTime(now.Year, now.Month, now.Day, input, 0, 0);
        return date < now
            ? date.AddDays(1)
            : date;
    }
}