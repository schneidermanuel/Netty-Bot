using System;
using System.Collections.Generic;

namespace DiscordBot.Modules.AutoMod.Rules;

internal class GuildMessageObservation
{
    private Dictionary<ulong, UserObservation> _userObservations;

    public GuildMessageObservation()
    {
        _userObservations = new Dictionary<ulong, UserObservation>();
    }

    public int RegisterMessage(ulong userId, int resetTime)
    {
        if (_userObservations.ContainsKey(userId))
        {
            var userObservation = _userObservations[userId];
            if (userObservation.LastMessage > DateTime.Now.AddSeconds(-1 * resetTime))
            {
                userObservation.MessageCount = 0;
            }

            userObservation.LastMessage = DateTime.Now;
            userObservation.MessageCount++;
            return userObservation.MessageCount;
        }

        _userObservations.Add(userId, new UserObservation
        {
            LastMessage = DateTime.Now,
            MessageCount = 1
        });
        return 1;
    }
}