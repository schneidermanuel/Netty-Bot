using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.ZenQuote;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.ZenQuote;

public class ZenQuoteTask : ITimedAction
{
    private readonly IZenQuoteDomain _domain;

    public ZenQuoteTask(IZenQuoteDomain domain)
    {
        _domain = domain;
    }

    public ExecutionTime GetExecutionTime()
    {
        return ExecutionTime.Daily;
    }

    public async Task ExecuteAsync(DiscordSocketClient client)
    {
        var registrations = await _domain.LoadAllRegistrations();
        var quote = await _domain.RetrieveQuoteOfTheDayAsync();
        foreach (var registration in registrations)
        {
            try
            {
                var channel =
                    (ISocketMessageChannel) client.GetGuild(registration.GuildId)
                        .GetChannel(registration.Channelid);
                await channel.SendMessageAsync(quote);
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                await _domain.RemoveRegistrationAsync(registration.Id);
            }
        }
    }
}