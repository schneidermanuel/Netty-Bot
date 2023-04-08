using System.Reflection;

namespace DiscordBot.Framework.Contract.Modularity;

public class CommandInfo
{
    public MethodInfo MethodInfo { get; set; }
    public ICommandModule CommandModule { get; set; }
}