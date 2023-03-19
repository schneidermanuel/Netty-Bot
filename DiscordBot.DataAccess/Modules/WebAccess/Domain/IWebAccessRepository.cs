using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Modules.WebAccess.Domain;

internal interface IWebAccessRepository
{
    Task<string> GetValue(string key);
    Task UpdateValue(string key, string value);
}