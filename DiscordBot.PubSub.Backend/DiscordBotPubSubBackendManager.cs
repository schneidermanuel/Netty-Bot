using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;
using DiscordBot.Framework.Contract;
using DiscordBot.Framework.Contract.Modularity;
using DiscordBot.PubSub.Backend.Data;
using DiscordBot.PubSub.Backend.Data.Guild;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace DiscordBot.PubSub.Backend;

internal class DiscordBotPubSubBackendManager : IDiscordBotPubSubBackendManager
{
    private readonly DiscordSocketClient _client;
    private readonly IEnumerable<ICommandModule> _modules;
    private readonly IModuleDataAccess _dataAccess;
    private Func<YoutubeNotification, Task> _callback;

    public DiscordBotPubSubBackendManager(DiscordSocketClient client, IEnumerable<ICommandModule> modules,
        IModuleDataAccess dataAccess)
    {
        _client = client;
        _modules = modules;
        _dataAccess = dataAccess;
    }

    public void Run(Func<YoutubeNotification, Task> callback)
    {
        _callback = callback;
        var builder = WebApplication.CreateBuilder();
        builder.Services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });
        var app = builder.Build();


        app.MapGet("/", ProcessGet);
        app.MapPost("/", ProcessPost);
        app.MapGet("/GuildStatus", ProcessGuilds);
        app.MapGet("/Guild/", ProcessGuild);


        var thread = new Thread(() => app.Run($"https://{BotClientConstants.Hostname}:{BotClientConstants.Port}"));
        thread.Start();
    }

    private async Task ProcessGuild(HttpContext context)
    {
        var guildId = context.Request.Query["guildId"];
        var userId = context.Request.Query["userId"];

        var guild = _client.Guilds.Single(g => g.Id == ulong.Parse(guildId));
        var user = guild.GetUser(ulong.Parse(userId));
        if (!user.GuildPermissions.Administrator)
        {
            context.Response.StatusCode = 401;
            await context.Response.CompleteAsync();
        }

        var moduleIdentifiers = _modules.Select(module => module.ModuleUniqueIdentifier);
        var modules = new List<Module>();
        foreach (var module in moduleIdentifiers)
        {
            var isEnabled = await _dataAccess.IsModuleEnabledForGuild(guild.Id, module);
            modules.Add(new Module
            {
                Enabled = isEnabled,
                UniqueKey = module
            });
        }

        var responseGuild = new Guild
        {
            Name = guild.Name,
            IconUrl = guild.IconUrl,
            Modules = modules
        };

        var json = JsonConvert.SerializeObject(responseGuild);
        await Responsd(context, json);
        await context.Response.CompleteAsync();
    }

    private async Task ProcessGuilds(HttpContext context)
    {
        try
        {
            var guildId = context.Request.Query["guildId"];
            var userId = context.Request.Query["userId"];
            if (_client.Guilds.All(guild => !guild.Id.ToString().Equals(guildId.ToString())))
            {
                await Responsd(context, "NotAdded");
                await context.Response.CompleteAsync();
                return;
            }

            var guild = _client.Guilds.Single(guild => guild.Id.ToString().Equals(guildId.ToString()));
            if (guild.GetUser(ulong.Parse(userId)).GuildPermissions.Administrator)
            {
                await Responsd(context, "Normal");
            }
            else
            {
                await Responsd(context, "MissingPermission");
            }

            await context.Response.CompleteAsync();
        }
        catch
        {
            context.Response.StatusCode = 400;
            await context.Response.CompleteAsync();
        }
    }

    private async Task ProcessPost(HttpContext context)
    {
        try
        {
            var stream = context.Request.Body;
            var data = ConvertAtomToSyndication(stream);
            if (data != null)
            {
                if (data.IsNewVideo)
                {
                    await _callback.Invoke(data);
                }

                context.Response.StatusCode = 200;
                await context.Response.CompleteAsync();
                return;
            }

            context.Response.StatusCode = 400;
            await context.Response.CompleteAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            context.Response.StatusCode = 500;
            await context.Response.CompleteAsync();
        }
    }

    private async Task ProcessGet(HttpContext context)
    {
        try
        {
            var re = context.Request;
            var challenge = re.Query["hub.challenge"];

            if (!string.IsNullOrEmpty(challenge))
            {
                await Responsd(context, challenge);
            }

            await context.Response.CompleteAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static async Task Responsd(HttpContext context, StringValues response)
    {
        var bytes = Encoding.UTF8.GetBytes(response);
        await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
    }

    private YoutubeNotification ConvertAtomToSyndication(Stream stream)
    {
        using (var xmlReader = XmlReader.Create(stream))
        {
            var feed = SyndicationFeed.Load(xmlReader);
            var item = feed.Items.FirstOrDefault();
            return new YoutubeNotification()
            {
                ChannelId = GetElementExtensionValueByOuterName(item, "channelId"),
                VideoId = GetElementExtensionValueByOuterName(item, "videoId"),
                Title = item?.Title.Text,
                Published = item?.PublishDate.ToString("dd/MM/yyyy"),
                Updated = item?.LastUpdatedTime.ToString("dd/MM/yyyy")
            };
        }
    }

    private string GetElementExtensionValueByOuterName(SyndicationItem item, string outerName)
    {
        return item.ElementExtensions.All(x => x.OuterName != outerName)
            ? null
            : item.ElementExtensions.Single(x => x.OuterName == outerName).GetObject<XElement>().Value;
    }
}