using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.GuildConfiguration;
using DiscordBot.DataAccess.Contract.UserConfiguration;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.Configuration;

internal class ConfigurationCommands : CommandModuleBase, ICommandModule
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
    
    [Command("language")]
    [Description("Sets the language for the bot.")]
    [Parameter(Name = "language", Description = "The language to set the bot to.", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.String)]
    public async Task ChangeLanguageAsync(SocketSlashCommand context)
    {
        await RequireArg(context, 1, "Supported Languages: \nGerman - de\nEnglish - en");
        var language = await RequireString(context);
        if (!_languages.Contains(language))
        {
            await context.RespondAsync("Language not supported\n\n");
            return;
        }

        await context.RespondAsync("🤝");
        await _userConfigurationBusinessLogic.SaveLanguageAsync(context.User.Id, language);
    }

    [Command("prefix")]
    [Description("Sets the prefix for the bot.")]
    [Parameter(Name = "prefix", Description = "The prefix to set the bot to.", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.String)]
    public async Task ChangePrefixAsync(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);

        await RequirePermissionAsync(context, guild, GuildPermission.Administrator);
        await RequireArg(context, 1, Localize(nameof(ConfigurationRessources.Error_NoPrefix)));
        var prefix = await RequireString(context);
        if (prefix.Length != 1)
        {
            await context.RespondAsync(Localize(nameof(ConfigurationRessources.Error_NoPrefix)));
            return;
        }

        var newPrefix = prefix[0];
        await _guildConfigBusinessLogic.SavePrefixAsync(guild.Id, newPrefix);
        await context.RespondAsync("🤝");
    }

    public override string ModuleUniqueIdentifier => "CONFIGURATION";
}