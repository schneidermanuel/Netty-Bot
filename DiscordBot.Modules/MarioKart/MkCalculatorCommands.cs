using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.MkCalculator;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.MarioKart;

internal class MkCalculatorCommands : CommandModuleBase, IGuildModule
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

    public override async Task<bool> CanExecuteAsync(ulong id, SocketCommandContext socketCommandContext)
    {
        return await IsEnabled(id);
    }

    [Command("race")]
    public async Task CalculateAsync(ICommandContext context)
    {
        await RequireArg(context, 6, Localize(nameof(MarioKartRessources.Error_Enter6Numbers)));
        var places = new List<int>();
        for (var i = 1; i <= 6; i++)
        {
            var place = await RequireIntArg(context, i);
            places.Add(place);
        }

        var comment = await RequireReminderOrEmpty(context, 7);

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

        await context.Channel.SendMessageAsync("", false, embedBuilder.Build());
    }

    [Command("mkrevert")]
    public async Task RevertGameAsync(SocketCommandContext context)
    {
        if (!await _manager.CanRevertAsync(context.Channel.Id))
        {
            await context.Channel.SendMessageAsync(Localize(nameof(MarioKartRessources.Error_NotRevertable)));
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

        await context.Channel.SendMessageAsync("", false, embedBuilder.Build());
    }

    private async Task<string> GetThumbnailUrlAsync(string comment)
    {
        var words = comment.Split(' ');
        var courseCode = words.FirstOrDefault()?.Trim()?.ToLower() ?? string.Empty;

        var url = $"https://www.mkleaderboards.com/images/mk8/{courseCode}.jpg";
        var client = new HttpClient();
        var response = await client.GetAsync(url);
        return response.IsSuccessStatusCode
            ? url
            : "https://www.kindpng.com/picc/m/494-4940057_mario-kart-8-icon-hd-png-download.png";
    }

    [Command("mkdisplay")]
    public async Task MkDisplayAsync(ICommandContext context)
    {
        await context.Channel.SendMessageAsync(
            "https://mk-leaderboard.netty-bot.com?key=" + context.Channel.Id);
    }

    [Command("mkcomplete")]
    public async Task FinishAsync(ICommandContext context)
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
        await context.Channel.SendMessageAsync(url);
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
    public async Task WorldReccordCommandAsync(ICommandContext context)
    {
        await RequireArg(context, 2, Localize(nameof(MarioKartRessources.Error_MkwrSyntax)));

        var strecke = await RequireString(context);
        var cc = await RequireIntArg(context, 2);

        var wr = await _worldRecordLoader.LoadWorldRecord(strecke, cc);
        if (wr == null)
        {
            await context.Channel.SendMessageAsync(Localize(nameof(MarioKartRessources.Error_CannotParseInformation)));
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

        await context.Channel.SendMessageAsync("", false, builder.Build());
    }

    public override async Task ExecuteAsync(ICommandContext context)
    {
        await ExecuteCommandsAsync(context);
    }

    protected override Type RessourceType => typeof(MarioKartRessources);
    public override string ModuleUniqueIdentifier => "MK CALC";
}