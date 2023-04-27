using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.MkCalculator;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.MarioKart;

internal class MarioKartModalListener : IModalListener
{
    private readonly MkGameManager _manager;

    public MarioKartModalListener(MkGameManager manager)
    {
        _manager = manager;
    }

    public string ButtonEventPrefix => "mkWarTeam";

    public async Task SubmittedAsync(ulong userId, SocketModal modal)
    {
        var customId = modal.Data.CustomId;
        if (customId.StartsWith("mkWarTeam_Active"))
        {
            var channelId = ulong.Parse(customId.Split('_')[2]);
            var teamName = modal.Data.Components.Single(c => c.CustomId == "teamName").Value;
            var teamImage = modal.Data.Components.Single(c => c.CustomId == "teamImage").Value;
            var enemyName = modal.Data.Components.Single(c => c.CustomId == "enemyName").Value;
            var enemyImage = modal.Data.Components.Single(c => c.CustomId == "enemyImage").Value;

            var game = new MkGame
            {
                Enemy = new MkTeam
                {
                    Image = enemyImage,
                    Name = enemyName
                },
                Team = new MkTeam
                {
                    Image = teamImage,
                    Name = teamName
                },
                GameId = 0
            };
            await _manager.StartGameAsync(channelId, game);
        }
    }
}