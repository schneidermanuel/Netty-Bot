using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.ReactionRoles;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.ReactionRoles;

public class ReactionRoleCommands : CommandModuleBase, IGuildModule
{
    private readonly IModuleDataAccess _dataAccess;
    private readonly ReactionRoleManager _manager;
    private readonly IReactionRoleBusinessLogic _businessLogic;

    public ReactionRoleCommands(IModuleDataAccess dataAccess, ReactionRoleManager manager,
        IReactionRoleBusinessLogic businessLogic) : base(dataAccess)
    {
        _dataAccess = dataAccess;
        _manager = manager;
        _businessLogic = businessLogic;
    }

    protected override Type RessourceType => typeof(ReactionRoleRessources);
    public override string ModuleUniqueIdentifier => "REACTION_ROLE";

    public override async Task<bool> CanExecuteAsync(ulong id, SocketCommandContext socketCommandContext)
    {
        await RequirePermissionAsync(socketCommandContext, GuildPermission.Administrator);
        return await IsEnabled(id);
    }

    public override Task ExecuteAsync(ICommandContext context)
    {
        return ExecuteCommandsAsync(context);
    }

    [Command("addReactionRole")]
    public async Task AddReactionRoleAsync(ICommandContext context)
    {
        if (context.Message.ReferencedMessage == null)
        {
            await context.Channel.SendMessageAsync(Localize(nameof(ReactionRoleRessources.Error_NotReplied)));
            return;
        }

        var emote = GetEmote(await RequireString(context));
        var roleId = await RequireUlongAsync(context, 2);

        var referencedMessage = context.Message.ReferencedMessage;
        var messageId = referencedMessage.Id;
        var channelId = referencedMessage.Channel.Id;

        var role = context.Guild.GetRole(roleId);
        if (role == null)
        {
            await context.Channel.SendMessageAsync(
                string.Format(Localize(nameof(ReactionRoleRessources.Error_InvalidRole)), roleId));
            return;
        }

        if (!await _businessLogic.CanAddReactionRoleAsync(referencedMessage.Id, emote))
        {
            await context.Channel.SendMessageAsync(Localize(nameof(ReactionRoleRessources.Error_EmoteAlreadyAdded)));
            return;
        }

        await referencedMessage.AddReactionAsync(emote);
        var reactionRole = new ReactionRole
        {
            Emote = emote,
            Id = 0,
            ChannelId = channelId,
            GuildId = context.Guild.Id,
            MessageId = messageId,
            RoleId = role.Id
        };
        _manager.ReactionRoles.Add(reactionRole);
        await _businessLogic.SaveReactionRoleAsync(reactionRole);
        await context.Message.DeleteAsync();
    }

    [Command("registerReactionRole")]
    public async Task RegisterReactionRoleAsync(ICommandContext context)
    {
        var prefix = await _dataAccess.GetServerPrefixAsync(context.Guild.Id);
        await RequireArg(context, 3, string.Format(Localize(nameof(ReactionRoleRessources.Error_SyntaxError)), prefix));
        var emote = GetEmote(await RequireString(context));
        var roleId = await RequireUlongAsync(context, 2);

        var role = context.Guild.GetRole(roleId);
        if (role == null)
        {
            await context.Channel.SendMessageAsync(
                string.Format(Localize(nameof(ReactionRoleRessources.Error_InvalidRole)), roleId));
            return;
        }

        var content = await RequireReminderArg(context, 3);
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
            GuildId = context.Guild.Id,
            MessageId = message.Id,
            RoleId = role.Id
        };
        _manager.ReactionRoles.Add(reactionRole);
        await _businessLogic.SaveReactionRoleAsync(reactionRole);
        await context.Message.DeleteAsync();
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