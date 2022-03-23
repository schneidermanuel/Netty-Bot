using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DiscordBot.Framework.Contract;
using DiscordBot.Framework.Contract.Boot;

namespace DiscordBot.Framework.BootSteps;

public class ConfigurationBootStep : IBootStep
{
    public async Task BootAsync()
    {
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/config.xml";
        var serializer = new XmlSerializer(typeof(MainConfig));
        var stream = new FileStream(path, FileMode.Open);
        var config = serializer.Deserialize(stream) as MainConfig;
        BotClientConstants.BotToken = config?.DiscordBotToken;
        await Task.CompletedTask;
    }

    public BootOrder StepPosition => BootOrder.First;
}