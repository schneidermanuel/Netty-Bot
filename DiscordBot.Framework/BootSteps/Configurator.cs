using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using DiscordBot.Framework.Contract;

namespace DiscordBot.Framework.BootSteps;

public static class Configurator
{
    public static void Configure()
    {
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/config.xml";
        var serializer = new XmlSerializer(typeof(MainConfig));
        var stream = new FileStream(path, FileMode.Open);
        var config = serializer.Deserialize(stream) as MainConfig;
        BotClientConstants.BotToken = config?.DiscordBotToken;
        BotClientConstants.TwitchClientId = config?.TwitchClientId;
        BotClientConstants.TwitchClientSecret = config?.TwitchClientSecret;
        BotClientConstants.Hostname = config?.Hostname;
        BotClientConstants.LavalinkHost = config?.LavalinkHost;
        BotClientConstants.LavalinkPassword = config?.LavalinkPassword;
        BotClientConstants.LavalinkSsl = (config?.LavalinkSsl).GetValueOrDefault();
        BotClientConstants.LavalinkPort = (config?.LavalinkPort).GetValueOrDefault(443);
        BotClientConstants.YoutubeApiKey = (config?.YoutubeApiKey);
        BotClientConstants.Port = (config?.Port).GetValueOrDefault();
    }
}