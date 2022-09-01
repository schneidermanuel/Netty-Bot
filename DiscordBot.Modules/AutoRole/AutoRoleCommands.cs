using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.AutoRole;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.AutoRole;

internal class AutoRoleCommands : CommandModuleBase, ICommandModule
{
    private readonly IAutoRoleBusinessLogic _businessLogic;
    private readonly AutoRoleManager _manager;

    public AutoRoleCommands(IModuleDataAccess dataAccess, IAutoRoleBusinessLogic businessLogic, AutoRoleManager manager)
        : base(dataAccess)
    {
        _businessLogic = businessLogic;
        _manager = manager;
    }
    
    [Command("autoRole-delete")]
    [Description("Delete an auto role setup")]
    [Parameter(Name = "role", Description = "The role to delete", IsOptional = false, ParameterType = ApplicationCommandOptionType.Role)]
    public async Task DeleteAutoRoleSetupAsync(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);
        var role = await RequireRoleAsync(context);
        var setups = (await _businessLogic.RetrieveAllSetupsForGuildAsync(guild.Id)).ToArray();
        if (setups.All(setup => setup.RoleId != role.Id))
        {
            await context.RespondAsync(string.Format(Localize(nameof(AutoRoleRessources.Error_InvalidRole)),
                role));
            return;
        }

        var setupToDelete = setups.Single(setup => setup.RoleId == role.Id);
        await _businessLogic.DeleteSetupAsync(setupToDelete.AutoRoleSetupId);
        await context.RespondAsync(Localize(nameof(AutoRoleRessources.Message_DeletedRegistration)));
        await _manager.RefreshSetupsAsync();
    }

    [Command("autoRole-list")]
    [Description("Lists all auto roles")]
    public async Task ListAutoRolesAsync(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);

        var setups = await _businessLogic.RetrieveAllSetupsForGuildAsync(guild.Id);
        var output = setups.Select(autoRoleSetup =>
                new { Role = guild.GetRole(autoRoleSetup.RoleId), Id = autoRoleSetup.RoleId })
            .Aggregate(string.Empty, (current, role) => current + $"{role.Role?.Name ?? "missingRole"} ({role.Id})\n");

        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithDescription(output);
        embedBuilder.WithTitle(Localize(nameof(AutoRoleRessources.Title_AutomaticDistributedRoles)));
        embedBuilder.WithCurrentTimestamp();
        embedBuilder.WithColor(Color.Blue);
        var embed = embedBuilder.Build();
        await context.RespondAsync("", new[] { embed });
    }

    [Command("autoRole-add")]
    [Description("Assignes a role to every user joinig the server")]
    [Parameter(Name = "role", Description = "The role to assign", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.Role)]
    public async Task AddAutoRoleAsync(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);
        await RequirePermissionAsync(context, guild, GuildPermission.ManageRoles);
        var role = await RequireRoleAsync(context);
        if (role == null)
        {
            await context.RespondAsync(
                string.Format(Localize(nameof(AutoRoleRessources.Error_RoleNotFound))));
            return;
        }

        if (role.Permissions.Administrator)
        {
            await context.RespondAsync(Localize(nameof(AutoRoleRessources.Error_PermissionTooHigh)));
            return;
        }

        var guildId = guild.Id;

        var canCreateSetup = await _businessLogic.CanCreateAutoRoleAsync(guildId, role.Id);
        if (!canCreateSetup)
        {
            await context.RespondAsync(Localize(nameof(AutoRoleRessources.Message_NewAutoRole)));
            return;
        }

        var setup = new AutoRoleSetup
        {
            AutoRoleSetupId = 0,
            GuildId = guildId,
            RoleId = role.Id
        };
        await _businessLogic.SaveSetupAsync(setup);
        await _manager.RefreshSetupsAsync();
        await context.RespondAsync("🤝");
    }


    protected override Type RessourceType => typeof(AutoRoleRessources);

    public override string ModuleUniqueIdentifier => "AUTOROLE";
}