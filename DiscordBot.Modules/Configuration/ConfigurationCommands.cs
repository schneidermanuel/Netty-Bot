using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.UserConfiguration;
using DiscordBot.DataAccess.Modules.UserConfiguration.BusinessLogic;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.Configuration;

internal class ConfigurationCommands : CommandModuleBase, IGuildModule
{
    private readonly IUserConfigurationBusinessLogic _userConfigurationBusinessLogic;
    private static string[] _languages = { "de", "en" };
    
    public ConfigurationCommands(IModuleDataAccess dataAccess, IUserConfigurationBusinessLogic userConfigurationBusinessLogic) : base(dataAccess)
    {
        _userConfigurationBusinessLogic = userConfigurationBusinessLogic;
    }

    protected override Type RessourceType => null;

    public override Task<bool> CanExecuteAsync(ulong id, SocketCommandContext socketCommandContext)
    {
        return Task.FromResult(true);
    }

    public override async Task ExecuteAsync(ICommandContext context)
    {
        await ExecuteCommandsAsync(context);
    }

    [Command("language")]
    public async Task ChangeLanguageAsync(ICommandContext context)
    {
        var language = await RequireString(context);
        if (!_languages.Contains(language))
        {
            await context.Channel.SendMessageAsync("Language not supported");
            return;
        }
        await context.Message.AddReactionAsync(Emoji.Parse("🤝"));
        await _userConfigurationBusinessLogic.SaveLanguageAsync(context.User.Id, language);

    }

    public override string ModuleUniqueIdentifier => "CONFIGURATION";
}