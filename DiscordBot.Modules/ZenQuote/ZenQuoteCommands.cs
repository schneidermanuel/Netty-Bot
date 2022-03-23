using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.ZenQuote;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.ZenQuote;

public class ZenQuoteCommands : CommandModuleBase, IGuildModule
{
    private readonly IZenQuoteBusinessLogic _businessLogic;

    public ZenQuoteCommands(IModuleDataAccess dataAccess, IZenQuoteBusinessLogic businessLogic) : base(dataAccess)
    {
        _businessLogic = businessLogic;
    }

    public override string ModuleUniqueIdentifier => "ZENQUOTE";

    public override async Task<bool> CanExecuteAsync(ulong id, SocketCommandContext socketCommandContext)
    {
        await RequirePermissionAsync(socketCommandContext, GuildPermission.Administrator);
        return await IsEnabled(id);
    }

    public override async Task ExecuteAsync(ICommandContext context)
    {
        await ExecuteCommandsAsync(context);
    }

    [Command("registerQuote")]
    public async Task RegisterForZenQuote(ICommandContext context)
    {
        await RequirePermissionAsync(context, GuildPermission.Administrator);

        var registrations = await _businessLogic.LoadAllRegistrations();
        if (registrations.Any(reg => reg.Channelid == context.Channel.Id && reg.GuildId == context.Guild.Id))
        {
            await context.Channel.SendMessageAsync("Auf diesem Kanal werden bereits tägliche Zitate versendet. ");
            return;
        }

        var channelId = context.Channel.Id;
        var guildId = context.Guild.Id;
        var registration = new ZenQuoteRegistration
        {
            Channelid = channelId,
            Id = 0,
            GuildId = guildId
        };
        await _businessLogic.SaveRegistrationAsync(registration);
        await context.Channel.SendMessageAsync("Auf diesem Kanal werden nun tägliche Motivationszitate versendet!");
    }

    [Command("unregisterQuote")]
    public async Task UnregisterQuoteAsync(ICommandContext context)
    {
        await RequirePermissionAsync(context, GuildPermission.Administrator);
        var registrations = await _businessLogic.LoadAllRegistrations();
        var regForChannel = registrations.Where(reg => reg.GuildId == context.Guild.Id &&
                                                       reg.Channelid == context.Channel.Id).ToList();
        if (!regForChannel.Any())
        {
            await context.Channel.SendMessageAsync("Auf diesem Kanal werden keine Zitate versendet.");
            return;
        }

        await _businessLogic.RemoveRegistrationAsync(regForChannel.Single().Id);
        await context.Channel.SendMessageAsync("Ok, ab nun werden hier keine Zitate mehr versendet. ");
    }
}