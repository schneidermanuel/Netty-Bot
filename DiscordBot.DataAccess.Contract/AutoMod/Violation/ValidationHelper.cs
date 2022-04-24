namespace DiscordBot.DataAccess.Contract.AutoMod.Violation;

public static class ValidationHelper
{
    public const string DoNothingKey = "DO_NOTHING";
    public const string WarnUserKey = "WARN";
    public const string DeleteMessageKey = "DELETE MESSAGE";
    public const string ActionKey = "VIOLATE_ACTION";

    public static IRuleViolationAction MapValidation(string key, string reason = null)
    {
        switch (key)
        {
            case DoNothingKey:
                return new DoNothingAction();
            case WarnUserKey:
                return new WarnUserAction(reason);
            case DeleteMessageKey:
                return new DeleteMessageAction();
            default:
                return new DoNothingAction();
        }
    }
}

