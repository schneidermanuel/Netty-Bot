using System.Threading.Tasks;
using Discord.Commands;
using DiscordBot.DataAccess.Contract;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.Basics;

public class BasicCommandModule : CommandModuleBase, IGuildModule
{
    public BasicCommandModule(IModuleDataAccess dataAccess) : base(dataAccess)
    {
    }

    public override string ModuleUniqueIdentifier => "BASICS";

    [Command("test")]
    public async Task TestCommand(ICommandContext context)
    {
        await context.Channel.SendMessageAsync("Hallo");
    }


    public override async Task<bool> CanExecuteAsync(ulong id, SocketCommandContext socketCommandContext)
    {
        return await IsEnabled(id);
    }

    public override async Task ExecuteAsync(ICommandContext context)
    {
        await ExecuteCommandsAsync(context);
    }
}