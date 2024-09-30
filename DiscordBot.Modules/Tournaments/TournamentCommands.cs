using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.Tournaments.Domain;
using DiscordBot.Framework.Contract.Modularity;
using DiscordBot.Framework.Contract.Modularity.Commands;

namespace DiscordBot.Modules.Tournaments;

public class TournamentCommands : CommandModuleBase, ICommandModule
{
    private readonly ITournamentsDomain _domain;

    public TournamentCommands(IModuleDataAccess dataAccess,
        ITournamentsDomain domain) : base(dataAccess)
    {
        _domain = domain;
    }

    [Command("join-tournament")]
    [Description("Joins to a tournament")]
    [Parameter(Name = "Code", Description = "The Code of the tournament", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.String)]
    [Parameter(Name = "Friendcode", Description = "Your Switch Friendcode", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.String)]
    [Parameter(Name = "CanHost", Description = "Can Host", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.Boolean)]
    public async Task JoinTournamentAsync(SocketSlashCommand context)
    {
        await context.DeferAsync();
        var code = await RequireString(context);
        var guild = await RequireGuild(context);
        var canJoin = await _domain.CanJoinTournamentAsync(context.User.Id, guild.Id, code);
        if (!canJoin.CanJoin)
        {
            await context.ModifyOriginalResponseAsync(x => x.Content = Localize(canJoin.Reason));
            return;
        }

        var friendcode = await RequireString(context, 2);
        var canHost = await RequireBool(context, 3);

        await _domain.JoinTournamentAsync(context.User.Id, context.User.Username, code, friendcode, canHost);
        await context.ModifyOriginalResponseAsync(x => x.Content = "🤝");
    }

    protected override Type RessourceType => typeof(TournamentResources);
    public override string ModuleUniqueIdentifier => "TOURNAMENTMANAGER";
}