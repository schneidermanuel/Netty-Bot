using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.Moderation;

internal class ModerationCommands : CommandModuleBase, ICommandModule
{
    public ModerationCommands(IModuleDataAccess dataAccess) : base(dataAccess)
    {
    }


    [Command("clean")]
    [Parameter(Name = "count", Description = "The count of messages to delete. default is 100",
        ParameterType = ApplicationCommandOptionType.Integer, IsOptional = true)]
    public async Task CleanAsync(SocketSlashCommand context)
    {
        var count = RequireIntArgOrDefault(context, 100);
        var guild = await RequireGuild(context);
        await RequirePermissionAsync(context, guild, GuildPermission.Administrator);

        var channel = (ITextChannel)context.Channel;
        var messages = (await channel.GetMessagesAsync(count).ToListAsync()).SelectMany(x => x);
        await channel.DeleteMessagesAsync(messages);
    }

    protected override Type RessourceType => typeof(ModerationResources);
    public override string ModuleUniqueIdentifier => "MODERATION";
}