using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DiscordBot.Framework.Contract;
using DiscordBot.PubSub.Backend.Data;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace DiscordBot.PubSub.Backend;

internal class DiscordBotPubSubBackendManager : IDiscordBotPubSubBackendManager
{
    private Func<YoutubeNotification, Task> _callback;

    public void Run(Func<YoutubeNotification, Task> callback)
    {
        _callback = callback;
        var builder = WebApplication.CreateBuilder();
        builder.Services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });
        var app = builder.Build();


        app.MapGet("/", ProcessGet);
        app.MapPost("/", ProcessPost);


        var thread = new Thread(() => app.Run($"https://{BotClientConstants.Hostname}:{BotClientConstants.Port}"));
        thread.Start();
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
                var bytes = Encoding.UTF8.GetBytes(challenge);
                await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
            }

            await context.Response.CompleteAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
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