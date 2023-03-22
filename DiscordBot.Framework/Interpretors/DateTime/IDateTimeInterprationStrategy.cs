using System;

namespace DiscordBot.Framework.Interpretors.DateTime;

internal interface IDateTimeInterprationStrategy
{
    bool CanInterpret(string data);
    int Priority { get; }
    System.DateTime Interpret(string data);
}