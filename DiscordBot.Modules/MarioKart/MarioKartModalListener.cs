using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.MarioKart;

internal class MarioKartModalListener : IModalListener
{
    public string ButtonEventPrefix => "mkWarTeam";

    public async Task SubmittedAsync(ulong userId, SocketModal modal)
    {
        var customId = modal.Data.CustomId;
        if (customId.StartsWith("mkWarTeam_Active"))
        {
            
        }
    }
}