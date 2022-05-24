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

    public MkCalculatorCommands(IModuleDataAccess dataAccess, IMkCalculator calculator) : base(dataAccess)
    {
        _calculator = calculator;
    }

    public override async Task<bool> CanExecuteAsync(ulong id, SocketCommandContext socketCommandContext)
    {
        return await IsEnabled(id);
    }

    [Command("mkRace")]
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
        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithColor(Color.Gold);
        embedBuilder.WithCurrentTimestamp();
        embedBuilder.WithTitle("Mario Kart Result");
        embedBuilder.WithThumbnailUrl(
            "https://www.kindpng.com/picc/m/494-4940057_mario-kart-8-icon-hd-png-download.png");
        embedBuilder.WithDescription(
            $"Team - Difference  Enemy\n{result.Points} - {result.Difference} - {result.EnemyPoints}");
        await context.Channel.SendMessageAsync("", false, embedBuilder.Build());
    }

    public override async Task ExecuteAsync(ICommandContext context)
    {
        await ExecuteCommandsAsync(context);
    }

    public override string ModuleUniqueIdentifier => "MK CALC";
}