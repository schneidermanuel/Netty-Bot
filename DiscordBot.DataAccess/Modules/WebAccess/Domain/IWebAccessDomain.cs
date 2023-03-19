using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Modules.WebAccess.Domain;

public interface IWebAccessDomain
{
    Task GenerateNewGuid();
    Task<string> GetCurrentGuid();
}