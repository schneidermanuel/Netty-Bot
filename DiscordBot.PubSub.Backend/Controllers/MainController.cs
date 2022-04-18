using Microsoft.AspNetCore.Mvc;

namespace DiscordBot.PubSub.Backend.Controllers;

[ApiController]
public class MainController : ControllerBase
{
    public MainController()
    {
        Console.WriteLine("Controller instanntiated");
    }
    [HttpGet]
    [Route("")]
    public async Task GetAsync()
    {
        Console.WriteLine(Request.QueryString);
        await Response.CompleteAsync();
    }

    [HttpPost]
    [Route("")]
    public async Task PostAsync()
    {
        Console.WriteLine(Request.QueryString);
        await Response.CompleteAsync();
    }
}