namespace DiscordBot.Framework.Contract.Interpretors.DateTime;

public interface IDateTimeInterpretor
{
    System.DateTime? Interpret(string data);
}