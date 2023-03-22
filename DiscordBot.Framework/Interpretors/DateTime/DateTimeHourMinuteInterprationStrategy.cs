using System.Linq;

namespace DiscordBot.Framework.Interpretors.DateTime;

internal class DateTimeHourMinuteInterprationStrategy : IDateTimeInterprationStrategy
{
    public bool CanInterpret(string data)
    {
        data = data.Replace(":", "");
        if (data.Length != 4)
        {
            return false;
        }

        if (int.TryParse(data.Take(2).ToArray(), out var hour) && int.TryParse(data.Skip(2).ToArray(), out var minute))
        {
            return hour is >= 0 and < 24 && minute is >= 0 and < 60;
        }

        return false;
    }

    public int Priority => 5;

    public System.DateTime Interpret(string data)
    {
        data = data.Replace(":", "");

        var hourin = int.Parse(data.Take(2).ToArray());
        var minutein = int.Parse(data.Skip(2).ToArray());
        var now = System.DateTime.Now;

        var date = new System.DateTime(now.Year, now.Month, now.Day, hourin, minutein, 0);
        return date < now
            ? date.AddDays(1)
            : date;
    }
}