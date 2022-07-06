using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.Huebcraft;

public class HuebcraftCommands : CommandModuleBase, IGuildModule
{
    public HuebcraftCommands(IModuleDataAccess dataAccess) : base(dataAccess)
    {
    }

    public override async Task<bool> CanExecuteAsync(ulong id, SocketCommandContext socketCommandContext)
    {
        await Task.CompletedTask;
        return id == 948632879389351956;
    }

    public override async Task ExecuteAsync(ICommandContext context)
    {
        await ExecuteCommandsAsync(context);
    }

    protected override Type RessourceType => null;
    public override string ModuleUniqueIdentifier => "HUEBCRAFT";

    [Command("bewerbung")]
    public async Task BewerbungCommand(ICommandContext context)
    {
        var bewerbungType = await RequireReminderArg(context);
        var channelName = new string($"{context.User.Username} {bewerbungType} Bewerbung".Take(90).ToArray());
        var channel = await context.Guild.CreateTextChannelAsync(channelName, properties =>
        {
            properties.CategoryId = 948687562694873129;
            properties.PermissionOverwrites = new Optional<IEnumerable<Overwrite>>(new List<Overwrite>
            {
                new(context.User.Id, PermissionTarget.User, new OverwritePermissions(PermValue.Deny,
                    PermValue.Deny, PermValue.Allow, PermValue.Allow
                    , PermValue.Allow, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Allow,
                    PermValue.Allow)),
                new(948633042094792734, PermissionTarget.Role,
                    new OverwritePermissions(PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow,
                        PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow,
                        PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow,
                        PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow,
                        PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow,
                        PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow,
                        PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow)),
                new(948632879389351956, PermissionTarget.Role, new OverwritePermissions(PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue.Deny,PermValue
                    .Deny))
            });
        });
        var teamRole = context.Guild.GetRole(948633042094792734);
        await channel.SendMessageAsync($"{teamRole.Mention}: Start der Bewerbung von {context.User.Mention} als {bewerbungType}");
    }

    [Command("fertig")]
    public async Task FertigCommand(ICommandContext context)
    {
        await RequirePermissionAsync(context, GuildPermission.Administrator);
        var channel = (SocketGuildChannel)context.Channel;
        await channel.DeleteAsync();
    }
}
