using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.DataAccess.Contract;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.MkCalculator;

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
        await RequireArg(context, 6, "Bitte geben Sie 6 Zahlen ein");
        var places = new List<int>();
        for (var i = 1; i <= 6; i++)
        {
            var place = await RequireIntArg(context, i);
            places.Add(place);
        }

        var result = _calculator.Calculate(places);
        _manager.RegisterResult(result, context.Guild.Id);
        var sumResult = _manager.GetFinalResult(context.Guild.Id);

        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithColor(Color.Gold);
        embedBuilder.WithCurrentTimestamp();
        embedBuilder.WithTitle("Mario Kart Result");
        embedBuilder.WithThumbnailUrl(
            "https://www.kindpng.com/picc/m/494-4940057_mario-kart-8-icon-hd-png-download.png");
        embedBuilder.WithDescription(
            $"Team - Difference - Enemy\nThis Round: {result.Points} - {result.Difference} - {result.EnemyPoints}\nTotal: {sumResult.Points} - {sumResult.Difference} - {sumResult.EnemyPoints}");
        await context.Channel.SendMessageAsync("", false, embedBuilder.Build());
    }

    [Command("mkcomplete")]
    public async Task FinishAsync(ICommandContext context)
    {
        var result = _manager.GetFinalResult(context.Guild.Id);
        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithColor(Color.Gold);
        embedBuilder.WithCurrentTimestamp();
        embedBuilder.WithTitle("Mario Kart Final Result");
        embedBuilder.WithThumbnailUrl(
            "https://www.kindpng.com/picc/m/494-4940057_mario-kart-8-icon-hd-png-download.png");
        embedBuilder.WithDescription(
            $"Team - Difference - Enemy\n{result.Points} - {result.Difference} - {result.EnemyPoints}");
        await context.Channel.SendMessageAsync("", false, embedBuilder.Build());
        _manager.EndGame(context.Guild.Id);
    }

    [Command("mkwr")]
    public async Task WorldReccordCommandAsync(ICommandContext context)
    {
        await RequireArg(context, 2, "Bitte geben Sie Strecke und die CC an! (!mkwr  MKS 150)");

        var strecke = await RequireString(context);
        var cc = await RequireIntArg(context, 2);

        var wr = await _worldRecordLoader.LoadWorldRecord(strecke, cc);
        if (wr == null)
        {
            await context.Channel.SendMessageAsync("Fehler bei der Datenübertragung");
            return;
        }

        var builder = new EmbedBuilder();
        builder.WithCurrentTimestamp();
        builder.WithColor(Color.Red);
        builder.WithTitle($"World Record: {wr.Trackname} - {wr.Time} - {wr.Player}");
        builder.WithThumbnailUrl(wr.Nation);
        builder.AddField("Lap 1", wr.Lap1, true);
        builder.AddField("Lap 2", wr.Lap2, true);
        builder.AddField("Lap 3", wr.Lap3, true);
        builder.AddField("Character", wr.Character, true);
        builder.AddField("Kart", wr.Kart, true);
        builder.AddField("Tires", wr.Tires, true);
        builder.AddField("Glider", wr.Gilder, true);
        builder.WithUrl(wr.VideoUrl);

        await context.Channel.SendMessageAsync("", false, builder.Build());
    }

    public override async Task ExecuteAsync(ICommandContext context)
    {
        await ExecuteCommandsAsync(context);
    }

    public override string ModuleUniqueIdentifier => "MK CALC";
}