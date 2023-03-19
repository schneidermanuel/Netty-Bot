using System;
using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Modules.WebAccess.Domain;

internal class WebAccessDomain : IWebAccessDomain
{
    private readonly IWebAccessRepository _repository;

    public WebAccessDomain(IWebAccessRepository repository)
    {
        _repository = repository;
    }
    public async Task GenerateNewGuid()
    {
        await _repository.UpdateValue("CURRENT_GUID", Guid.NewGuid().ToString());
    }

    public async Task<string> GetCurrentGuid()
    {
        return await _repository.GetValue("CURRENT_GUID");
    }
}