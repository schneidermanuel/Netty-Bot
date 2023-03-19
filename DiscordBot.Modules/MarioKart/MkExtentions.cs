namespace DiscordBot.Modules.MarioKart;

internal static class MkExtentions
{
    public static string To3CharString(this int i)
    {
        if (i < 10)
            return i + "  ";
        if (i < 100)
            return i + " ";
        return i.ToString();
    }
}