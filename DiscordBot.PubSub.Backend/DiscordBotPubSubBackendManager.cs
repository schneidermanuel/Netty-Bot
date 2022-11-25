using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Discord;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.ReactionRoles;
using DiscordBot.DataAccess.Contract.ZenQuote;
using DiscordBot.DataAccess.Modules.WebAccess.Domain;
using DiscordBot.Framework.Contract;
using DiscordBot.Framework.Contract.Modularity;
using DiscordBot.Framework.Contract.Modules.AutoMod;
using DiscordBot.Framework.Contract.Modules.AutoMod.Rules;
using DiscordBot.Framework.Contract.Modules.AutoRole;
using DiscordBot.Framework.Contract.Modules.ReactionRoles;
using DiscordBot.Framework.Contract.Modules.YoutubeRegistrations;
using DiscordBot.PubSub.Backend.Data;
using DiscordBot.PubSub.Backend.Data.Guild;
using DiscordBot.PubSub.Backend.Data.Guild.AutoModRule;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using ReactionRole = DiscordBot.PubSub.Backend.Data.Guild.ReactionRole.ReactionRole;

namespace DiscordBot.PubSub.Backend;

internal class DiscordBotPubSubBackendManager : IDiscordBotPubSubBackendManager
{
    private readonly DiscordSocketClient _client;
    private readonly IEnumerable<ICommandModule> _modules;
    private readonly IEnumerable<IGuildAutoModRule> _autoModRules;
    private readonly IWebAccessDomain _webAccessDomain;
    private readonly IModuleDataAccess _dataAccess;
    private readonly IAutoModRefresher _autoModRefresher;
    private readonly IAutoRoleRefresher _autoRoleRefresher;
    private readonly IReactionRoleDomain _reactionRoleDomain;
    private readonly IReactionRoleRefresher _reactionRoleRefresher;
    private readonly IYoutubeRefresher _youtubeRefresher;
    private Func<YoutubeNotification, Task> _callback;

    public DiscordBotPubSubBackendManager(DiscordSocketClient client,
        IEnumerable<ICommandModule> modules,
        IEnumerable<IGuildAutoModRule> autoModRules,
        IWebAccessDomain webAccessDomain,
        IModuleDataAccess dataAccess,
        IAutoModRefresher autoModRefresher,
        IAutoRoleRefresher autoRoleRefresher,
        IReactionRoleDomain reactionRoleDomain,
        IReactionRoleRefresher reactionRoleRefresher,
        IYoutubeRefresher youtubeRefresher)
    {
        _client = client;
        _modules = modules;
        _autoModRules = autoModRules;
        _webAccessDomain = webAccessDomain;
        _dataAccess = dataAccess;
        _autoModRefresher = autoModRefresher;
        _autoRoleRefresher = autoRoleRefresher;
        _reactionRoleDomain = reactionRoleDomain;
        _reactionRoleRefresher = reactionRoleRefresher;
        _youtubeRefresher = youtubeRefresher;
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
        app.MapGet("/Modules/AutoModConfig/listConfigs", AutoModListConfigs);
        app.MapPut("/Modules/Refresh/AutoMod", RefreshAutoMod);
        app.MapGet("/Guild/Roles", ProcessGuildRoles);
        app.MapPut("/AutoRole", RefreshAutoRole);
        app.MapGet("/Guild/ReactionRoles", ProcessReactionRoles);
        app.MapGet("/Guild/Channels", ProcessChannels);
        app.MapGet("/Guild/Emoji", ProcessEmoji);
        app.MapPut("/Modules/Refresh/ReactionRole", RefreshReactionRole);
        app.MapPut("/Modules/Refresh/Youtube", RefreshYoutube);

        var thread = new Thread(() => app.Run($"https://{BotClientConstants.Hostname}:{BotClientConstants.Port}"));
        thread.Start();
    }

    private async Task RefreshYoutube(HttpContext context)
    {
        await RequireAuthenticationAsync(context);

        var guildId = ulong.Parse(context.Request.Query["guildId"]);
        await _youtubeRefresher.RefreshGuildAsync(guildId);
    }

    private async Task RefreshReactionRole(HttpContext context)
    {
        await RequireAuthenticationAsync(context);

        var guildId = ulong.Parse(context.Request.Query["guildId"]);
        await _reactionRoleRefresher.RefreshAsync(guildId);
    }

    private async Task ProcessEmoji(HttpContext context)
    {
        await RequireAuthenticationAsync(context);

        var guildId = ulong.Parse(context.Request.Query["guildId"]);
        var guild = _client.GetGuild(guildId);

        var emoji = guild.Emotes.Select(emote => new Data.Guild.ReactionRole.Emoji
        {
            Id = emote.Id,
            Name = emote.ToString(),
            Url = emote.Url
        });
        await Responsd(context, JsonConvert.SerializeObject(emoji));
    }

    private async Task ProcessChannels(HttpContext context)
    {
        await RequireAuthenticationAsync(context);

        var guildId = ulong.Parse(context.Request.Query["guildId"]);
        var guild = _client.GetGuild(guildId);

        var textChannel = guild.TextChannels.Select(channel => new Channel
        {
            ChannelId = channel.Id.ToString(),
            ChannelName = channel.Name
        });
        await Responsd(context, JsonConvert.SerializeObject(textChannel));
    }

    private async Task ProcessReactionRoles(HttpContext context)
    {
        await RequireAuthenticationAsync(context);

        var guildId = ulong.Parse(context.Request.Query["guildId"]);
        var reactionRoles = await _reactionRoleDomain.RetrieveReactionRolesForGuildAsync(guildId);
        var list = new List<ReactionRole>();
        var guild = _client.GetGuild(guildId);
        foreach (var reactionRole in reactionRoles)
        {
            try
            {
                await MapReactionRolesAsync(guild, reactionRole, list);
            }
            catch (Exception e)
            {
                // Ignored for now
            }
        }

        await Responsd(context, JsonConvert.SerializeObject(list));
    }

    private static async Task MapReactionRolesAsync(SocketGuild guild,
        DataAccess.Contract.ReactionRoles.ReactionRole reactionRole, List<ReactionRole> list)
    {
        var channel = guild.GetChannel(reactionRole.ChannelId);
        var channelName = channel.Name;
        var message = await ((SocketTextChannel)channel).GetMessageAsync(reactionRole.MessageId);
        var content = message.Content;
        if (message.Embeds.Any())
        {
            content += $"\n{message.Embeds.First().Description}";
        }

        if (reactionRole.Emote is Emote guildEmote)
        {
            var role = new ReactionRole
            {
                Id = reactionRole.Id,
                ChannelName = channelName,
                MessageContent = content,
                IsUrlEmote = true,
                UnicodeEmote = null,
                Url = guildEmote.Url,
                RoleId = reactionRole.RoleId
            };
            list.Add(role);
        }
        else if (reactionRole.Emote is Emoji emoji)
        {
            var role = new ReactionRole
            {
                Id = reactionRole.Id,
                ChannelName = channelName,
                MessageContent = content,
                IsUrlEmote = false,
                Url = null,
                UnicodeEmote = emoji.ToString(),
                RoleId = reactionRole.RoleId
            };
            list.Add(role);
        }
    }

    private async Task RefreshAutoRole(HttpContext context)
    {
        await RequireAuthenticationAsync(context);
        var guildId = ulong.Parse(context.Request.Query["guildId"]);
        await _autoRoleRefresher.RefreshAsync(guildId);
    }

    private async Task ProcessGuildRoles(HttpContext context)
    {
        await RequireAuthenticationAsync(context);
        var guildIdString = context.Request.Query["guildId"];
        var guildId = ulong.Parse(guildIdString);
        var socketRoles = _client.GetGuild(guildId)?.Roles ?? new List<SocketRole>();
        var roles = socketRoles
            .Where(role => !role.IsManaged)
            .Select(role => new GuildRole
            {
                Id = role.Id.ToString(),
                Name = role.Name,
                IsAdmin = role.Permissions.Administrator,
            });
        var json = JsonConvert.SerializeObject(roles);
        await Responsd(context, json);
        await context.Response.CompleteAsync();
    }

    private async Task RefreshAutoMod(HttpContext context)
    {
        await RequireAuthenticationAsync(context);
        var guildIdString = context.Request.Query["guildId"];
        var guildId = ulong.Parse(guildIdString);
        await context.Response.CompleteAsync();

        await _autoModRefresher.RefreshGuildAsync(guildId);
    }

    private async Task RequireAuthenticationAsync(HttpContext context)
    {
        if (context.Request.Headers.ContainsKey("X-Verification-Guid"))
        {
            var headerGuild = context.Request.Headers["X-Verification-Guid"].ToString();
            var databaseGuid = await _webAccessDomain.GetCurrentGuid();

            if (!headerGuild.Equals(databaseGuid))
            {
                context.Response.StatusCode = 401;
                await context.Response.CompleteAsync();
                throw new UnauthorizedAccessException();
            }
        }
        else
        {
            context.Response.StatusCode = 401;
            await context.Response.CompleteAsync();
            throw new UnauthorizedAccessException();
        }
    }

    private async Task AutoModListConfigs(HttpContext context)
    {
        var keys = _autoModRules
            .Select(rule => new AutoModConfig
            {
                RuleKey = rule.RuleIdentifier,
                Configs = rule.GetConfigurations().Select(x => new AutoModConfigConfiguration
                {
                    Key = x.Key,
                    Type = x.Value.ToString()
                }).ToArray()
            })
            .ToArray();

        var json = JsonConvert.SerializeObject(keys);
        await Responsd(context, json);
        await context.Response.CompleteAsync();
    }

    private async Task ProcessGuild(HttpContext context)
    {
        await RequireAuthenticationAsync(context);
        try
        {
            var guildId = context.Request.Query["guildId"];
            var userId = context.Request.Query["userId"];

            if (_client.Guilds.All(guild => !guild.Id.ToString().Equals(guildId.ToString())))
            {
                await Responsd(context, "Guild does not exist", 400);
                await context.Response.CompleteAsync();
                return;
            }

            var guild = _client.Guilds.Single(guild => guild.Id.ToString().Equals(guildId.ToString()));
            var user = guild.GetUser(ulong.Parse(userId));
            if (!user.GuildPermissions.Administrator)
            {
                context.Response.StatusCode = 401;
                await context.Response.CompleteAsync();
                return;
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
        }
        catch (Exception e)
        {
            context.Response.StatusCode = 500;
        }
        finally
        {
            await context.Response.CompleteAsync();
        }
    }

    private async Task ProcessGuilds(HttpContext context)
    {
        await RequireAuthenticationAsync(context);

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

    public static Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }


    private async Task ProcessPost(HttpContext context)
    {
        try
        {
            var checksumHeader = context.Request.Headers["X-Hub-Signature"];
            var signature = checksumHeader.ToString().Split('=')[1];
            var stream = context.Request.Body;
            string body;
            using (var reader = new StreamReader(stream))
            {
                body = await reader.ReadToEndAsync();
            }

            var isValid = YoutubePubSubSecret.Check(body, signature);

            if (!isValid)
            {
                await context.Response.CompleteAsync();
                return;
            }

            await using (var newStream = GenerateStreamFromString(body))
            {
                var data = ConvertAtomToSyndication(newStream);
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

    private static async Task Responsd(HttpContext context, StringValues response, int statusCode = 200)
    {
        var bytes = Encoding.UTF8.GetBytes(response);
        context.Response.StatusCode = statusCode;
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