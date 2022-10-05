using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Discord.WebSocket;
using DiscordBot.Framework.Contract;
using DiscordBot.PubSub.Backend.Data;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace DiscordBot.PubSub.Backend;

internal class DiscordBotPubSubBackendManager : IDiscordBotPubSubBackendManager
{
    private readonly DiscordSocketClient _client;
    private Func<YoutubeNotification, Task> _callback;

    public DiscordBotPubSubBackendManager(DiscordSocketClient client)
    {
        _client = client;
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


        var thread = new Thread(() => app.Run($"https://{BotClientConstants.Hostname}:{BotClientConstants.Port}"));
        thread.Start();
    }

    private async Task ProcessGuilds(HttpContext context)
    {
        try
        {
            var guildId = context.Request.Query["guildId"];
            var userId = context.Request.Query["userId"];
            if (_client.Guilds.All(guild => guild.Id != guildId))
            {
                await Responsd(context, "NotAdded");
                await context.Response.CompleteAsync();
                return;
            }

            var guild = _client.Guilds.Single(guild => guild.Id == guildId);
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