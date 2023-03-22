using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.Event;
using DiscordBot.Framework.Contract.Interpretors.DateTime;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.Event;

internal class EventCommands : CommandModuleBase
{
    private readonly IDateTimeInterpretor _timeInterpretor;
    private readonly IEventDomain _eventDomain;

    public EventCommands(IModuleDataAccess dataAccess, IDateTimeInterpretor timeInterpretor, IEventDomain eventDomain) :
        base(dataAccess)
    {
        _timeInterpretor = timeInterpretor;
        _eventDomain = eventDomain;
    }

    protected override Type RessourceType => typeof(EventResources);
    public override string ModuleUniqueIdentifier => "EVENT";

    [Command("event")]
    [Description("Creates a Event in the channel")]
    [Parameter(ParameterType = ApplicationCommandOptionType.String, IsOptional = false, Name = "time",
        Description = "when the event takes place")]
    [Parameter(ParameterType = ApplicationCommandOptionType.Integer, IsOptional = true, Name = "maxusers",
        Description = "the max amount of users in the event")]
    [Parameter(ParameterType = ApplicationCommandOptionType.String, IsOptional = true, Name = "name",
        Description = "the name for the event")]
    [Parameter(ParameterType = ApplicationCommandOptionType.Role, IsOptional = true, Name = "role",
        Description = "Assign a Role to all participants")]
    public async Task EventCommand(SocketSlashCommand context)
    {
        var timeString = await RequireString(context);
        var eventTime = _timeInterpretor.Interpret(timeString);
        if (eventTime == null)
        {
            await context.RespondAsync(Localize(nameof(EventResources.Error_TimeNotResolved)));
            return;
        }

        var maxUsers = GetOptionalIntParameter(context, "maxusers");
        var name = GetOptionalStringParameter(context, "name") ??
                   string.Format(Localize(nameof(EventResources.EventTitle)), eventTime.Value.ToString("dd.MM hh:mm"));
        var role = GetOptionalRoleParameter(context, "role");
        var guild = await RequireGuild(context);

        await CreateEventAsync(name, eventTime.Value, maxUsers, role, guild, context);
    }

    private async Task CreateEventAsync(string name, DateTime time, int? maxUsers, IRole role, IGuild guild,
        SocketSlashCommand context)
    {
        var embed = new EmbedBuilder()
            .WithAuthor(context.User.Username)
            .WithColor(Color.Blue)
            .WithCurrentTimestamp()
            .WithTitle(name)
            .WithDescription(time.ToString("dd.MM hh:mm"))
            .AddField(string.Format(Localize(nameof(EventResources.Field_Can)), 0.ToString()), "-")
            .AddField(string.Format(Localize(nameof(EventResources.Field_Cant)), 0.ToString()), "-")
            .AddField(string.Format(Localize(nameof(EventResources.Field_Unsure)), 0.ToString()), "-")
            .Build();


        await context.RespondAsync("🤝");
        var message = await context.Channel.SendMessageAsync(string.Empty, false, embed);
        var e = new DataAccess.Contract.Event.Event
        {
            RoleId = role?.Id,
            ChannelId = message.Channel.Id,
            MessageId = message.Id,
            AutoDeleteDate = time,
            GuildId = guild.Id,
            MaxUsers = maxUsers
        };
        await _eventDomain.SaveAsync(e);
    }
}