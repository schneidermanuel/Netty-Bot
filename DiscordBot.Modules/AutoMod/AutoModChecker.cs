using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.AutoMod;

internal class AutoModChecker : CommandModuleBase, IGuildModule
{
    private readonly AutoModManager _manager;

    public AutoModChecker(IModuleDataAccess dataAccess, AutoModManager manager) : base(dataAccess)
    {
        _manager = manager;
    }

    public override async Task<bool> CanExecuteAsync(ulong id, SocketCommandContext socketCommandContext)
    {
        return await IsEnabled(id);
    }

    public override async Task ExecuteAsync(ICommandContext context)
    {
        if (context.User is SocketGuildUser { GuildPermissions.Administrator: false })
        {
            await _manager.ProcessMessage(context);
        }
    }

    protected override Type RessourceType => typeof(AutoModRessources);
    public override string ModuleUniqueIdentifier => "AUTOMOD_EXECUTOR";
}