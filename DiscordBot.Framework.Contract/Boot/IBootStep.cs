using System.Threading.Tasks;

namespace DiscordBot.Framework.Contract.Boot;

public interface IBootStep
{
    Task BootAsync();
    BootOrder StepPosition { get; }
}