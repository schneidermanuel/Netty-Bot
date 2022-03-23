using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.ZenQuote;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.ZenQuote;

public class ZenQuoteTask : ITimedAction
{
    private readonly IZenQuoteBusinessLogic _businessLogic;

    public ZenQuoteTask(IZenQuoteBusinessLogic businessLogic)
    {
        _businessLogic = businessLogic;
    }

    public ExecutionTime GetExecutionTime()
    {
        return ExecutionTime.Daily;
    }

    public async Task Execute(DiscordSocketClient client)
    {
        var registrations = await _businessLogic.LoadAllRegistrations();
        var quote = await _businessLogic.RetrieveQuoteOfTheDayAsync();
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
                await _businessLogic.RemoveRegistrationAsync(registration.Id);
            }
        }
    }
}