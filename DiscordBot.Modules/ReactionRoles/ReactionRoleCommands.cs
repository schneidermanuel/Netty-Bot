using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.ReactionRoles;
using DiscordBot.Framework.Contract.Modularity;
using DiscordBot.Framework.Contract.Modularity.Commands;

namespace DiscordBot.Modules.ReactionRoles;

public class ReactionRoleCommands : CommandModuleBase, ICommandModule
{
    private readonly IModuleDataAccess _dataAccess;
    private readonly ReactionRoleManager _manager;
    private readonly IReactionRoleDomain _domain;

    public ReactionRoleCommands(IModuleDataAccess dataAccess, ReactionRoleManager manager,
        IReactionRoleDomain domain) : base(dataAccess)
    {
        _dataAccess = dataAccess;
        _manager = manager;
        _domain = domain;
    }

    protected override Type RessourceType => typeof(ReactionRoleRessources);
    public override string ModuleUniqueIdentifier => "REACTION_ROLE";

    [Command("addReactionRole")]
    [Description("Adds a Reaction Role to an existing Message")]
    [Parameter(Name = "emote", Description = "The emote to react with", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.String)]
    [Parameter(Name = "role", Description = "The role to assign", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.Role)]
    [Parameter(Name = "message", Description = "The ID of the Message. Rightclick => Copy ID", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.String)]
    public async Task AddReactionRoleAsync(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);

        await RequirePermissionAsync(context, guild, GuildPermission.ManageRoles);
        IMessage message = null;
        try
        {
            var messageId = await RequireUlongAsync(context, 3);
            message = await context.Channel.GetMessageAsync(messageId);
        }
        catch (Exception)
        {
            //Ignored
        }

        if (message == null)
        {
            await context.RespondAsync(Localize(nameof(ReactionRoleRessources.Error_MessageNotFound)));
            return;
        }

        var emote = GetEmote(await RequireString(context));
        var role = await RequireRoleAsync(context, 2);

        if (role == null)
        {
            await context.RespondAsync(
                string.Format(Localize(nameof(ReactionRoleRessources.Error_InvalidRole))));
            return;
        }

        if (!await _domain.CanAddReactionRoleAsync(message.Id, emote))
        {
            await context.RespondAsync(Localize(nameof(ReactionRoleRessources.Error_EmoteAlreadyAdded)));
            return;
        }

        await message.AddReactionAsync(emote);
        var reactionRole = new ReactionRole
        {
            Emote = emote,
            Id = 0,
            ChannelId = message.Channel.Id,
            GuildId = guild.Id,
            MessageId = message.Id,
            RoleId = role.Id
        };
        _manager.ReactionRoles.Add(reactionRole);
        await _domain.SaveReactionRoleAsync(reactionRole);
        await context.RespondAsync("🤝");
    }

    [Command("registerReactionRole")]
    [Description("Registers a Reaction Role to a new Message")]
    [Parameter(Name = "emote", Description = "The emote to react with", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.String)]
    [Parameter(Name = "role", Description = "The role to assign", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.Role)]
    [Parameter(Name = "message", Description = "The message to send", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.String)]
    public async Task RegisterReactionRoleAsync(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);

        await RequirePermissionAsync(context, guild, GuildPermission.ManageRoles);
        var prefix = await _dataAccess.GetServerPrefixAsync(guild.Id);
        await RequireArg(context, 3, string.Format(Localize(nameof(ReactionRoleRessources.Error_SyntaxError)), prefix));
        var emote = GetEmote(await RequireString(context));
        var role = await RequireRoleAsync(context, 2);

        if (role == null)
        {
            await context.RespondAsync(
                string.Format(Localize(nameof(ReactionRoleRessources.Error_InvalidRole))));
            return;
        }

        var content = await RequireString(context, 3);
        Embed embed = null;
        if (content.StartsWith("embed:"))
        {
            var prefixLength = "embed:".Length;
            content = content.Substring(prefixLength, content.Length - prefixLength);
            var builder = new EmbedBuilder();
            builder.WithDescription(content);
            content = string.Empty;
            builder.WithColor(Color.Green);
            embed = builder.Build();
        }

        var message = await context.Channel.SendMessageAsync(content, false, embed);
        await message.AddReactionAsync(emote);
        var reactionRole = new ReactionRole
        {
            Emote = emote,
            Id = 0,
            ChannelId = message.Channel.Id,
            GuildId = guild.Id,
            MessageId = message.Id,
            RoleId = role.Id
        };
        _manager.ReactionRoles.Add(reactionRole);
        await _domain.SaveReactionRoleAsync(reactionRole);
        await context.RespondAsync("🤝");
    }

    private IEmote GetEmote(string emote)
    {
        try
        {
            return Emote.Parse(emote);
        }
        catch (Exception)
        {
            return new Emoji(emote);
        }
    }
}