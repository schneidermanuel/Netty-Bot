using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.GuildConfiguration;
using DiscordBot.DataAccess.Contract.UserConfiguration;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.Configuration;

internal class ConfigurationCommands : CommandModuleBase, IGuildModule
{
    private readonly IUserConfigurationBusinessLogic _userConfigurationBusinessLogic;
    private readonly IGuildConfigBusinessLogic _guildConfigBusinessLogic;
    private static string[] _languages = { "de", "en" };

    public ConfigurationCommands(IModuleDataAccess dataAccess,
        IUserConfigurationBusinessLogic userConfigurationBusinessLogic,
        IGuildConfigBusinessLogic guildConfigBusinessLogic) : base(dataAccess)
    {
        _userConfigurationBusinessLogic = userConfigurationBusinessLogic;
        _guildConfigBusinessLogic = guildConfigBusinessLogic;
    }

    protected override Type RessourceType => typeof(ConfigurationRessources);

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
        await RequireArg(context, 1, "Supported Languages: \nGerman - de\nEnglish - en");
        var language = await RequireString(context);
        if (!_languages.Contains(language))
        {
            await context.Channel.SendMessageAsync("Language not supported\n\n");
            return;
        }

        await context.Message.AddReactionAsync(Emoji.Parse("🤝"));
        await _userConfigurationBusinessLogic.SaveLanguageAsync(context.User.Id, language);
    }

    [Command("prefix")]
    public async Task ChangePrefixAsync(ICommandContext context)
    {
        await RequireArg(context, 1, Localize(nameof(ConfigurationRessources.Error_NoPrefix)));
        var prefix = await RequireString(context);
        if (prefix.Length != 1)
        {
            await context.Channel.SendMessageAsync(Localize(nameof(ConfigurationRessources.Error_NoPrefix)));
            return;
        }

        var newPrefix = prefix[0];
        await _guildConfigBusinessLogic.SavePrefixAsync(context.Guild.Id, newPrefix);
        await context.Channel.SendMessageAsync(Localize(nameof(ConfigurationRessources.Message_PrefixChanged)));
    }

    public override string ModuleUniqueIdentifier => "CONFIGURATION";
}