using System;

namespace DiscordBot.Modules.AutoMod.Rules;

internal class UserObservation
{
    public DateTime LastMessage { get; set; }
    public int MessageCount { get; set; }
}