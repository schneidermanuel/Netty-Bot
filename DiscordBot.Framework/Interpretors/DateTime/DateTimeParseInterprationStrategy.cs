namespace DiscordBot.Framework.Interpretors.DateTime;

internal class DateTimeParseInterprationStrategy : IDateTimeInterprationStrategy
{
    public bool CanInterpret(string data)
    {
        return System.DateTime.TryParse(data, out _);
    }

    public int Priority => 1;
    public System.DateTime Interpret(string data)
    {
        return System.DateTime.Parse(data);
    }
}