using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.UserConfiguration;
using DiscordBot.Framework.Contract.Modularity;
using DiscordBot.Framework.Contract.Modularity.Commands;

namespace DiscordBot.Modules.Configuration;

internal class ConfigurationCommands : CommandModuleBase, ICommandModule
{
    private readonly IUserConfigurationDomain _userConfigurationDomain;
    private static string[] _languages = { "de", "en", "ch", "es" };

    public ConfigurationCommands(IModuleDataAccess dataAccess,
        IUserConfigurationDomain userConfigurationDomain) : base(dataAccess)
    {
        _userConfigurationDomain = userConfigurationDomain;
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
        await _userConfigurationDomain.SaveLanguageAsync(context.User.Id, language);
    }

    public override string ModuleUniqueIdentifier => "CONFIGURATION";
}