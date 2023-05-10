using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.MkCalculator;
using DiscordBot.Framework.Contract.Helper;
using DiscordBot.Framework.Contract.Modularity;
using DiscordBot.Framework.Contract.Modularity.Commands;

namespace DiscordBot.Modules.MarioKart;

internal class MkCalculatorCommands : CommandModuleBase, ICommandModule
{
    private readonly IMkCalculator _calculator;
    private readonly MkGameManager _gameManager;
    private readonly IMkWorldRecordLoader _worldRecordLoader;
    private readonly IMarioKartWarCacheDomain _warCacheDomain;
    private readonly IImageHelper _imageHelper;

    public MkCalculatorCommands(IModuleDataAccess dataAccess, IMkCalculator calculator, MkGameManager gameManager,
        IMkWorldRecordLoader worldRecordLoader, IMarioKartWarCacheDomain warCacheDomain,
        IImageHelper imageHelper) :
        base(dataAccess)
    {
        _calculator = calculator;
        _gameManager = gameManager;
        _worldRecordLoader = worldRecordLoader;
        _warCacheDomain = warCacheDomain;
        _imageHelper = imageHelper;
    }

    [Command("race")]
    [Description("Registers a race to the current Mario Kart War session")]
    [Parameter(Name = "Places", Description = "The space sepperated list of places", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.String)]
    [Parameter(Name = "map", Description = "The map plaied", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.String, IsAutocomplete = true)]
    [Parameter(Name = "Comment", Description = "A comment", IsOptional = true,
        ParameterType = ApplicationCommandOptionType.String)]
    public async Task CalculateAsync(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);
        var placesString = await RequireString(context);
        var comment = RequireStringOrEmpty(context, 3);
        var map = await RequireString(context, 2);
        if (map.Contains(' '))
        {
            map = map.Split(' ')[0];
        }

        List<int> places;
        try
        {
            places = placesString.Split(' ').Select(int.Parse).ToList();
        }
        catch (Exception)
        {
            await context.RespondAsync(Localize(nameof(MarioKartRessources.Error_Enter6Numbers)));
            return;
        }

        if (!_gameManager.HasChannelRunningGame(context.Channel.Id))
        {
            var modal = await BuildTeamSetupModalAsync(guild.Id);
            modal.WithCustomId($"mkWarTeam_Active_{context.Channel.Id}");
            await context.RespondWithModalAsync(modal.Build());
            var tempResult = _calculator.Calculate(places);
            tempResult.Map = map;
            tempResult.Comment = comment;
            await _gameManager.RegisterResultAsync(tempResult, context.Channel.Id);
            return;
        }

        await context.DeferAsync();
        var result = _calculator.Calculate(places);
        result.Map = map;
        result.Comment = comment;
        var resultId = await _gameManager.RegisterResultAsync(result, context.Channel.Id);

        if (!resultId.HasValue)
        {
            return;
        }

        _imageHelper.Screenshot(
            $"https://mk-leaderboard.netty-bot.com/v2/table.php?language={GetPreferedLanguage()}&raceId={resultId.Value}\"",
            ".table");
        await context.ModifyOriginalResponseAsync(option =>
        {
            option.Content = "🤝";
            option.Attachments =
                new Optional<IEnumerable<FileAttachment>>(new[] { new FileAttachment("screenshot.png") });
        });
    }

    [Command("mkSetTeams")]
    public async Task ChangeTeamsCommandAsync(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);
        var modal = await BuildTeamSetupModalAsync(guild.Id);
        modal.WithCustomId($"mkWarTeam_Inactive_{context.Channel.Id}");
        await context.RespondWithModalAsync(modal.Build());
    }

    private async Task<ModalBuilder> BuildTeamSetupModalAsync(ulong guildId)
    {
        var data = await _warCacheDomain.RetrieveCachedRegistryAsync(guildId);
        var builder = new ModalBuilder();
        builder.WithTitle("War setup");
        builder.AddTextInput("Team Name", "teamName", value: data.TeamName, required: false);
        builder.AddTextInput("Team Image", "teamImage", value: data.TeamImage, required: false);
        builder.AddTextInput("Enemy Name", "enemyName", value: data.EnemyName, required: false);
        builder.AddTextInput("Enemy Image", "enemyImage", value: data.EnemyImage, required: false);
        return builder;
    }


    [Command("mkrevert")]
    [Description("Reverts the last Mario Kart Race")]
    public async Task RevertGameAsync(SocketSlashCommand context)
    {
        if (!await _gameManager.CanRevertAsync(context.Channel.Id))
        {
            await context.RespondAsync(Localize(nameof(MarioKartRessources.Error_NotRevertable)));
            return;
        }

        await _gameManager.RevertGameAsync(context.Channel.Id);

        var result = _gameManager.RetrieveGame(context.Channel.Id).Totals;
        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithColor(Color.Gold);
        embedBuilder.WithCurrentTimestamp();
        embedBuilder.WithTitle("Reverted!");
        embedBuilder.WithThumbnailUrl(
            "https://www.kindpng.com/picc/m/494-4940057_mario-kart-8-icon-hd-png-download.png");
        embedBuilder.WithDescription(string.Format(Localize(nameof(MarioKartRessources.Message_FinalResult)),
            result.Points, result.Difference, result.EnemyPoints));

        await context.RespondAsync("", new[] { embedBuilder.Build() });
    }


    [Command("mkdisplay")]
    [Description(
        "Creates a link to use in your OBS Browser Source")]
    public async Task MkDisplayAsync(SocketSlashCommand context)
    {
        await context.Channel.SendMessageAsync(
            "https://mk-leaderboard.netty-bot.com?key=" + context.Channel.Id);
    }

    [Command("mkcomplete")]
    [Description("Completes the current Mario Kart War")]
    public async Task FinishAsync(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);
        var result = _gameManager.RetrieveGame(context.Channel.Id);
        await _gameManager.EndGameAsync(context.Channel.Id);
        var url = BuildChartUrl(result);
        await context.DeferAsync();
        _imageHelper.Screenshot(url, ".table");
        await context.ModifyOriginalResponseAsync(option =>
        {
            option.Content = "🤝";
            option.Attachments =
                new Optional<IEnumerable<FileAttachment>>(new[] { new FileAttachment("screenshot.png") });
        });
    }

    private string BuildChartUrl(MkGame game)
    {
        var url = "https://analytics.netty-bot.com/MarioKart/result.php?";
        var chartValue = 0;
        foreach (var race in game.Races)
        {
            chartValue += race.Points - race.EnemyPoints;
            url +=
                $"scoreHistory[]={chartValue}&comments[]={(race.Track + " " + race.Comment).Trim().Replace(" ", "%20")}&";
        }

        return url;
    }

    private string BuildFinalDescription(MkResult result, IReadOnlyCollection<MkHistoryItem> mkHistoryItems)
    {
        var desc = string.Format(Localize(nameof(MarioKartRessources.Message_FinalResult)),
            result.Points, result.Difference, result.EnemyPoints) + "\n\n";
        for (var i = 0; i < mkHistoryItems.Count; i++)
        {
            var item = mkHistoryItems.ElementAt(i);
            var comment = string.IsNullOrEmpty(item.Comment) ? string.Empty : $"({item.Comment})";
            desc +=
                $"{i + 1}: {item.TeamPoints} - {Math.Abs(item.TeamPoints - item.EnemyPoints)} - {item.EnemyPoints} {comment}\n";
        }

        return desc;
    }

    [Command("mkwr")]
    [Description("Displays the current WR of the selected Mario Kart course at either 150 or 200 ccm")]
    [Parameter(Name = "course", Description = "The course to display the WR for", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.String)]
    [Parameter(Name = "ccm", Description = "The ccm to display the WR for", IsOptional = true,
        ParameterType = ApplicationCommandOptionType.Integer)]
    public async Task WorldReccordCommandAsync(SocketSlashCommand context)
    {
        await RequireArg(context, 1, Localize(nameof(MarioKartRessources.Error_MkwrSyntax)));

        var strecke = await RequireString(context);
        var cc = RequireIntArgOrDefault(context, 2, 150);

        var wr = await _worldRecordLoader.LoadWorldRecord(strecke, cc);
        if (wr == null)
        {
            await context.RespondAsync(Localize(nameof(MarioKartRessources.Error_CannotParseInformation)));
            return;
        }

        var builder = new EmbedBuilder();
        builder.WithCurrentTimestamp();
        builder.WithColor(Color.Red);
        builder.WithTitle(string.Format(Localize(nameof(MarioKartRessources.WR_Title)), wr.Trackname, wr.Time,
            wr.Player));
        builder.WithThumbnailUrl(wr.Nation);
        builder.AddField(Localize(nameof(MarioKartRessources.WR_Lap1)), wr.Lap1, true);
        builder.AddField(Localize(nameof(MarioKartRessources.WR_Lap2)), wr.Lap2, true);
        builder.AddField(Localize(nameof(MarioKartRessources.WR_Lap3)), wr.Lap3, true);
        builder.AddField(Localize(nameof(MarioKartRessources.WR_Char)), wr.Character, true);
        builder.AddField(Localize(nameof(MarioKartRessources.WR_Kart)), wr.Kart, true);
        builder.AddField(Localize(nameof(MarioKartRessources.WR_Tires)), wr.Tires, true);
        builder.AddField(Localize(nameof(MarioKartRessources.WR_Glider)), wr.Gilder, true);
        if (!string.IsNullOrEmpty(wr.VideoUrl))
        {
            builder.WithUrl(wr.VideoUrl);
        }

        await context.RespondAsync("", new[] { builder.Build() });
    }

    protected override Type RessourceType => typeof(MarioKartRessources);
    public override string ModuleUniqueIdentifier => "MK CALC";
}