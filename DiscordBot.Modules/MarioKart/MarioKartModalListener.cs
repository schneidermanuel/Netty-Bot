using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.MkCalculator;
using DiscordBot.Framework.Contract.Helper;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.MarioKart;

internal class MarioKartModalListener : IModalListener
{
    private readonly MkGameManager _manager;
    private readonly IImageHelper _imageHelper;
    private readonly IModuleDataAccess _dataAccess;
    private readonly IMarioKartWarCacheDomain _warCacheDomain;

    public MarioKartModalListener(
        MkGameManager manager,
        IImageHelper imageHelper,
        IModuleDataAccess dataAccess,
        IMarioKartWarCacheDomain warCacheDomain)
    {
        _manager = manager;
        _imageHelper = imageHelper;
        _dataAccess = dataAccess;
        _warCacheDomain = warCacheDomain;
    }

    public string ButtonEventPrefix => "mkWarTeam";

    public async Task SubmittedAsync(ulong userId, SocketModal modal)
    {
        try
        {
            var customId = modal.Data.CustomId;
            var channelId = ulong.Parse(customId.Split('_')[2]);
            var teamName = modal.Data.Components.Single(c => c.CustomId == "teamName").Value;
            var teamImage = modal.Data.Components.Single(c => c.CustomId == "teamImage").Value;
            var enemyName = modal.Data.Components.Single(c => c.CustomId == "enemyName").Value;
            var enemyImage = modal.Data.Components.Single(c => c.CustomId == "enemyImage").Value;

            var channel = (IGuildChannel)modal.Channel;
            await _warCacheDomain.SaveTeamsAsync(new MarioKartWarRegistry(teamName, teamImage, enemyName, enemyImage),
                channel.GuildId);

            if (customId.StartsWith("mkWarTeam_Active"))
            {
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
                var raceId = await _manager.StartGameAsync(channelId, game);
                var language = await _dataAccess.GetUserLanguageAsync(modal.User.Id);
                _imageHelper.Screenshot(
                    $"https://mk-leaderboard.netty-bot.com/v2/table.php?language={language}&raceId={raceId}\"",
                    ".table");
                await modal.RespondWithFileAsync("screenshot.png");
            }
            else
            {
                await modal.RespondAsync("🤝");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}