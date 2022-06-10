using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.DataAccess.Contract;
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

        var result = _calculator.Calculate(places);
        _manager.RegisterResult(result, context.Channel.Id);
        var sumResult = _manager.GetFinalResult(context.Channel.Id);

        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithColor(Color.Gold);
        embedBuilder.WithCurrentTimestamp();
        embedBuilder.WithTitle("Mario Kart Result");
        embedBuilder.WithThumbnailUrl(
            "https://www.kindpng.com/picc/m/494-4940057_mario-kart-8-icon-hd-png-download.png");
        embedBuilder.WithDescription(string.Format(Localize(nameof(MarioKartRessources.Message_RaceResult)), result.Points, result.Difference, result.EnemyPoints, sumResult.Points, sumResult.Difference, sumResult.EnemyPoints));

        await context.Channel.SendMessageAsync("", false, embedBuilder.Build());
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
        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithColor(Color.Gold);
        embedBuilder.WithCurrentTimestamp();
        embedBuilder.WithTitle("Mario Kart Final Result");
        embedBuilder.WithThumbnailUrl(
            "https://www.kindpng.com/picc/m/494-4940057_mario-kart-8-icon-hd-png-download.png");
        embedBuilder.WithDescription(string.Format(Localize(nameof(MarioKartRessources.Message_FinalResult)), result.Points, result.Difference, result.EnemyPoints));

        await context.Channel.SendMessageAsync("", false, embedBuilder.Build());
        _manager.EndGame(context.Channel.Id);
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
        builder.WithTitle(string.Format(Localize(nameof(MarioKartRessources.WR_Title)), wr.Trackname, wr.Time, wr.Player));
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