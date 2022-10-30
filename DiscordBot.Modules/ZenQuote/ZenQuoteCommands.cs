using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.ZenQuote;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.ZenQuote;

public class ZenQuoteCommands : CommandModuleBase, ICommandModule
{
    private readonly IZenQuoteDomain _domain;

    public ZenQuoteCommands(IModuleDataAccess dataAccess, IZenQuoteDomain domain) : base(dataAccess)
    {
        _domain = domain;
    }

    protected override Type RessourceType => typeof(ZenQuoteRessources);
    public override string ModuleUniqueIdentifier => "ZENQUOTE";

    [Command("registerQuote")]
    [Description("Sends a quote to this channel once a day")]
    public async Task RegisterForZenQuote(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);

        await RequirePermissionAsync(context, guild, GuildPermission.Administrator);

        var registrations = await _domain.LoadAllRegistrations();
        if (registrations.Any(reg => reg.Channelid == context.Channel.Id && reg.GuildId == guild.Id))
        {
            await context.RespondAsync(Localize(nameof(ZenQuoteRessources.Error_QuotesAlreadyEnabled)));
            return;
        }

        var channelId = context.Channel.Id;
        var guildId = guild.Id;
        var registration = new ZenQuoteRegistration
        {
            Channelid = channelId,
            Id = 0,
            GuildId = guildId
        };
        await _domain.SaveRegistrationAsync(registration);
        await context.RespondAsync(Localize(nameof(ZenQuoteRessources.Message_QuoteEnabled)));
    }

    [Command("unregisterQuote")]
    [Description("Stops sending a quote to this channel")]
    public async Task UnregisterQuoteAsync(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);
        await RequirePermissionAsync(context, guild, GuildPermission.Administrator);
        var registrations = await _domain.LoadAllRegistrations();
        var regForChannel = registrations.Where(reg => reg.GuildId == guild.Id &&
                                                       reg.Channelid == context.Channel.Id).ToList();
        if (!regForChannel.Any())
        {
            await context.RespondAsync(Localize(nameof(ZenQuoteRessources.Error_QuotesAlreadyDisabled)));
            return;
        }

        await _domain.RemoveRegistrationAsync(regForChannel.Single().Id);
        await context.RespondAsync(Localize(nameof(ZenQuoteRessources.Message_QuoteDisabled)));
    }
}