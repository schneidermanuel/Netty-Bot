namespace DiscordBot.Framework.Extentions;

public static class ConvertExtentions
{
    public static int? ToInt(this string input)
    {
        if (int.TryParse(input, out var output))
        {
            return output;
        }

        return null;
    }
}