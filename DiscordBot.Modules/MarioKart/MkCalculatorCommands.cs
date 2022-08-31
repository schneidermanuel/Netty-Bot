using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.MkCalculator;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.MarioKart;

internal class MkCalculatorCommands : CommandModuleBase, ICommandModule
{
    private readonly IMkCalculator _calculator;
    private readonly MkManager _manager;
    private readonly IMkWorldRecordLoader _worldRecordLoader;

    public MkCalculatorCommands(IModuleDataAccess dataAccess, IMkCalculator calculator, MkManager manager,
        IMkWorldRecordLoader worldRecordLoader) :
        base(dataAccess)
    {
        _calculator = calculator;
        _manager = manager;
        _worldRecordLoader = worldRecordLoader;
    }

    [Command("race")]
    [Description("Registers a race to the current Mario Kart War session")]
    [Parameter(Name = "Player1", Description = "The Position of the Player (1-12)", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.Integer)]
    [Parameter(Name = "Player2", Description = "The Position of the Player (1-12)", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.Integer)]
    [Parameter(Name = "Player3", Description = "The Position of the Player (1-12)", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.Integer)]
    [Parameter(Name = "Player4", Description = "The Position of the Player (1-12)", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.Integer)]
    [Parameter(Name = "Player5", Description = "The Position of the Player (1-12)", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.Integer)]
    [Parameter(Name = "Player6", Description = "The Position of the Player (1-12)", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.Integer)]
    [Parameter(Name = "Comment", Description = "Use any comment you want", IsOptional = true,
        ParameterType = ApplicationCommandOptionType.String)]
    public async Task CalculateAsync(SocketSlashCommand context, IGuild guild)
    {
        await RequireArg(context, 6, Localize(nameof(MarioKartRessources.Error_Enter6Numbers)));
        var places = new List<int>();
        for (var i = 1; i <= 6; i++)
        {
            var place = await RequireIntArg(context, i);
            places.Add(place);
        }

        var comment = RequireStringOrEmpty(context, 7);

        var result = _calculator.Calculate(places);
        await _manager.RegisterResultAsync(result, context.Channel.Id, comment);
        var sumResult = _manager.GetFinalResult(context.Channel.Id);


        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithColor(Color.Gold);
        embedBuilder.WithCurrentTimestamp();
        embedBuilder.WithTitle("Mario Kart Result");
        embedBuilder.WithThumbnailUrl(await GetThumbnailUrlAsync(comment));
        embedBuilder.WithDescription(string.Format(Localize(nameof(MarioKartRessources.Message_RaceResult)),
            result.Points.To3CharString(), result.Difference.To3CharString(),
            result.EnemyPoints.To3CharString(),
            sumResult.Points.To3CharString(),
            sumResult.Difference.To3CharString(),
            sumResult.EnemyPoints.To3CharString()));

        await context.RespondAsync("", new[] { embedBuilder.Build() });
    }


    [Command("mkrevert")]
    [Description("Reverts the last Mario Kart Race")]
    public async Task RevertGameAsync(SocketSlashCommand context, IGuild guild)
    {
        if (!await _manager.CanRevertAsync(context.Channel.Id))
        {
            await context.RespondAsync(Localize(nameof(MarioKartRessources.Error_NotRevertable)));
            return;
        }

        await _manager.RevertGameAsync(context.Channel.Id);

        var result = _manager.GetFinalResult(context.Channel.Id);
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

    private async Task<string> GetThumbnailUrlAsync(string comment)
    {
        var words = comment.Split(' ');
        var courseCode = words.FirstOrDefault()?.Trim().ToLower() ?? string.Empty;

        var url = $"https://www.mkleaderboards.com/images/mk8/{courseCode}.jpg";
        var client = new HttpClient();
        var response = await client.GetAsync(url);
        return response.IsSuccessStatusCode
            ? url
            : "https://www.kindpng.com/picc/m/494-4940057_mario-kart-8-icon-hd-png-download.png";
    }

    [Command("mkdisplay")]
    [Description(
        "Creates a link to use in your OBS Browser Source")]
    public async Task MkDisplayAsync(SocketSlashCommand context, IGuild guild)
    {
        await context.Channel.SendMessageAsync(
            "https://mk-leaderboard.netty-bot.com?key=" + context.Channel.Id);
    }

    [Command("mkcomplete")]
    [Description("Completes the current Mario Kart War")]
    public async Task FinishAsync(SocketSlashCommand context, IGuild guild)
    {
        var result = _manager.GetFinalResult(context.Channel.Id);
        var games = (await _manager.RetriveHistoryAsync(result.GameId)).ToArray();
        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithColor(Color.Gold);
        embedBuilder.WithCurrentTimestamp();
        embedBuilder.WithTitle("Mario Kart Final Result");
        embedBuilder.WithThumbnailUrl(
            "https://www.kindpng.com/picc/m/494-4940057_mario-kart-8-icon-hd-png-download.png");
        embedBuilder.WithDescription(BuildFinalDescription(result, games));

        await context.Channel.SendMessageAsync("", false, embedBuilder.Build());
        _manager.EndGame(context.Channel.Id);
        var url = BuildChartUrl(games);
        await context.RespondAsync(url);
    }

    private string BuildChartUrl(IReadOnlyCollection<MkHistoryItem> games)
    {
        var url = "https://analytics.netty-bot.com/MarioKart/result.php?";
        var chartValue = 0;
        foreach (var game in games)
        {
            chartValue += game.TeamPoints - game.EnemyPoints;
            url += $"scoreHistory[]={chartValue}&comments[]={game.Comment.Replace(" ", "%20")}&";
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
    public async Task WorldReccordCommandAsync(SocketSlashCommand context, IGuild guild)
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