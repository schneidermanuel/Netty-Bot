namespace DiscordBot.DataAccess.Contract.Tournaments.Domain;

public class CanJoinResult
{
    public bool CanJoin { get; }
    public string Reason { get; }

    private CanJoinResult(bool canJoin, string reason = null)
    {
        CanJoin = canJoin;
        Reason = reason;
    }

    public static CanJoinResult Yes()
    {
        return new CanJoinResult(true);
    }

    public static CanJoinResult No(string reasonKey)
    {
        return new CanJoinResult(false, reasonKey);
    }
}