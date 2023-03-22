using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.Event;

namespace DiscordBot.Modules.Event;

internal class EventLifesycleManager
{
    private readonly IEventDomain _domain;
    private readonly DiscordSocketClient _client;

    private IList<DataAccess.Contract.Event.Event> _currentEvents;

    public EventLifesycleManager(IEventDomain domain, DiscordSocketClient client)
    {
        _domain = domain;
        _client = client;
        _currentEvents = new List<DataAccess.Contract.Event.Event>();
    }


    public void StartListenForChanges()
    {
        _client.ReactionAdded += ProcessReaction;
    }

    private async Task ProcessReaction(Cacheable<IUserMessage, ulong> message,
        Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
    {
        if (_currentEvents.Any(e => e.MessageId == message.Id))
        {
            switch (reaction.Emote.ToString())
            {
                case "✅":

                    break;
                case "❌":
                    break;

                case "❓":

                    break;
                case "🗑":
                    
                    break;
                default:
                    return;
            }
        }
    }


    public async Task Refresh()
    {
        var currentEvents = await _domain.GetAllCurrentEventsAsync();
        _currentEvents.Clear();
        foreach (var currentEvent in currentEvents)
        {
            _currentEvents.Add(currentEvent);
        }
    }
}