using System.Reflection;

namespace DiscordBot.Framework.Contract.Modularity.Commands;

public class CommandInfo
{
    public MethodInfo MethodInfo { get; set; }
    public ICommandModule CommandModule { get; set; }
}