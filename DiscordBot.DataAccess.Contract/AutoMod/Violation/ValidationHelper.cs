using System;

namespace DiscordBot.DataAccess.Contract.AutoMod.Violation;

public static class ValidationHelper
{
    public const string DoNothingKey = "DO_NOTHING";
    public const string WarnUserKey = "WARN";
    public const string DeleteMessageKey = "DELETE";
    public const string DeleteAndNotifyKey = "DELETE AND NOTIFY";
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
            case DeleteAndNotifyKey:
                return new DeleteAndNotifyAction(reason);
            default:
                return new DoNothingAction();
        }
    }

    public static string MapValueTypeToString(ConfigurationValueType type)
    {
        switch (type)
        {
            case ConfigurationValueType.Unavailable:
                return "Nicht Verfügbar";
            case ConfigurationValueType.BoolValueOnly:
                return "TRUE / FALSE";
            case ConfigurationValueType.IntValueOnly:
                return "Positive Ganzzahl";
            case ConfigurationValueType.AnyValue:
                return "Freitext";
            case ConfigurationValueType.ActionValue:
                return $"{DoNothingKey} / {DeleteMessageKey} / {DeleteAndNotifyKey} / {WarnUserKey}";
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}