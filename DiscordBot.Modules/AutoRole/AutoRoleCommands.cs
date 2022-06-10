using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.AutoRole;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.AutoRole;

internal class AutoRoleCommands : CommandModuleBase, IGuildModule
{
    private readonly IAutoRoleBusinessLogic _businessLogic;
    private readonly AutoRoleManager _manager;

    public AutoRoleCommands(IModuleDataAccess dataAccess, IAutoRoleBusinessLogic businessLogic, AutoRoleManager manager)
        : base(dataAccess)
    {
        _businessLogic = businessLogic;
        _manager = manager;
    }

    public override async Task<bool> CanExecuteAsync(ulong id, SocketCommandContext socketCommandContext)
    {
        var user = socketCommandContext.Guild.GetUser(socketCommandContext.User.Id);
        return await IsEnabled(id) && user.GuildPermissions.ManageRoles;
    }

    public override async Task ExecuteAsync(ICommandContext context)
    {
        await ExecuteCommandsAsync(context);
    }

    [Command("autoRoleConfig")]
    public async Task ConfigAutoRoleCommand(ICommandContext context)
    {
        var method = await RequireString(context);
        switch (method)
        {
            case "add":
                await AddAutoRoleAsync(context);
                await context.Message.AddReactionAsync(Emoji.Parse("🤝"));
                await _manager.RefreshSetupsAsync();
                break;
            case "list":
                await ListAutoRolesAsync(context);
                break;

            case "delete":
                await DeleteAutoRoleSetupAsync(context);
                await context.Message.AddReactionAsync(Emoji.Parse("🤝"));
                await _manager.RefreshSetupsAsync();
                break;
            default:
                await context.Channel.SendMessageAsync(string.Format(Localize(nameof(AutoRoleRessources.Error_InvalidAction)), method));
                break;
        }
    }

    private async Task DeleteAutoRoleSetupAsync(ICommandContext context)
    {
        var roleId = await RequireUlongAsync(context, 2);
        var setups = (await _businessLogic.RetrieveAllSetupsForGuildAsync(context.Guild.Id)).ToArray();
        if (setups.All(setup => setup.RoleId != roleId))
        {
            await context.Channel.SendMessageAsync(string.Format(Localize(nameof(AutoRoleRessources.Error_InvalidRole)), roleId));
            return;
        }

        var setupToDelete = setups.Single(setup => setup.RoleId == roleId);
        await _businessLogic.DeleteSetupAsync(setupToDelete.AutoRoleSetupId);
        await context.Channel.SendMessageAsync(Localize(nameof(AutoRoleRessources.Message_DeletedRegistration)));
    }

    private async Task ListAutoRolesAsync(ICommandContext context)
    {
        var setups = await _businessLogic.RetrieveAllSetupsForGuildAsync(context.Guild.Id);
        var output = setups.Select(autoRoleSetup =>
                new { Role = context.Guild.GetRole(autoRoleSetup.RoleId), Id = autoRoleSetup.RoleId })
            .Aggregate(string.Empty, (current, role) => current + $"{role.Role?.Name ?? "missingRole"} ({role.Id})\n");

        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithDescription(output);
        embedBuilder.WithTitle(Localize(nameof(AutoRoleRessources.Title_AutomaticDistributedRoles)));
        embedBuilder.WithCurrentTimestamp();
        embedBuilder.WithColor(Color.Blue);
        var embed = embedBuilder.Build();
        await context.Channel.SendMessageAsync("", false, embed);
    }

    private async Task AddAutoRoleAsync(ICommandContext context)
    {
        var roleId = await RequireUlongAsync(context, 2);
        var role = context.Guild.GetRole(roleId);
        if (role == null)
        {
            await context.Channel.SendMessageAsync(string.Format(Localize(nameof(AutoRoleRessources.Error_RoleNotFound)), roleId));
            return;
        }

        if (role.Permissions.Administrator)
        {
            await context.Channel.SendMessageAsync(Localize(nameof(AutoRoleRessources.Error_PermissionTooHigh)));
            return;
        }
        var guildId = context.Guild.Id;

        var canCreateSetup = await _businessLogic.CanCreateAutoRoleAsync(guildId, roleId);
        if (!canCreateSetup)
        {
            await context.Channel.SendMessageAsync(Localize(nameof(AutoRoleRessources.Message_NewAutoRole)));
            return;
        }

        var setup = new AutoRoleSetup
        {
            AutoRoleSetupId = 0,
            GuildId = guildId,
            RoleId = roleId
        };
        await _businessLogic.SaveSetupAsync(setup);
    }

    protected override Type RessourceType => typeof(AutoRoleRessources);
    public override string ModuleUniqueIdentifier => "AUTOROLE";
}