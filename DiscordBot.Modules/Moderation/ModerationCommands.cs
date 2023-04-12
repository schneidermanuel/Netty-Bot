using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;
using DiscordBot.Framework.Contract.Modularity;
using DiscordBot.Framework.Contract.Modularity.Commands;

namespace DiscordBot.Modules.Moderation;

internal class ModerationCommands : CommandModuleBase, ICommandModule
{
    public ModerationCommands(IModuleDataAccess dataAccess) : base(dataAccess)
    {
    }


    [Command("clean")]
    [Description("cleans n messages from the channel")]
    [Parameter(Name = "count", Description = "The amount of messages to delete",
        ParameterType = ApplicationCommandOptionType.Integer, IsOptional = true)]
    public async Task CleanAsync(SocketSlashCommand context)
    {
        var count = RequireIntArgOrDefault(context, 1, 100);
        var guild = await RequireGuild(context);
        await RequirePermissionAsync(context, guild, GuildPermission.Administrator);

        await context.RespondAsync("cleaning...");
        var channel = (ITextChannel)context.Channel;
        var messages =
            (await channel.GetMessagesAsync(count).FlattenAsync()).Where(message =>
                message.Timestamp.DateTime.AddDays(14) > DateTime.Now);
        await channel.DeleteMessagesAsync(messages);
        await context.DeleteOriginalResponseAsync();
    }

    protected override Type RessourceType => typeof(ModerationResources);
    public override string ModuleUniqueIdentifier => "MODERATION";
}