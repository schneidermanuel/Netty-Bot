using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.Event;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.Event;

internal class EventLifesycleButtonListener : IButtonListener
{
    private readonly IEventDomain _domain;
    private readonly DiscordSocketClient _client;
    private readonly IModuleDataAccess _dataAccess;

    public EventLifesycleButtonListener(IEventDomain domain, DiscordSocketClient client, IModuleDataAccess dataAccess)
    {
        _domain = domain;
        _client = client;
        _dataAccess = dataAccess;
    }


    private async Task UserUnsureAsync(ulong userId, DataAccess.Contract.Event.Event relevantEvent, ulong messageId,
        ulong channelId)
    {
        var guild = _client.GetGuild(relevantEvent.GuildId);
        var user = guild.GetUser(userId);
        var message = (RestUserMessage)await guild
            .GetTextChannel(channelId)
            .GetMessageAsync(messageId);
        var embed = message.Embeds.Single();

        var canField = embed.Fields.Single(f => f.Name.Contains('✅'));
        var unsureField = embed.Fields.Single(f => f.Name.Contains('❓'));
        var cantField = embed.Fields.Single(f => f.Name.Contains('❌'));
        var subField = embed.Fields.SingleOrDefault(f => f.Name.Contains("🔼"));
        if (unsureField.Value.Contains(user.Mention))
        {
            return;
        }

        var unsureTags = unsureField.Value.Split('\n')
            .Where(tag => tag.Trim().StartsWith("<"))
            .Where(tag => tag != user.Mention).ToList();
        var cantTags = cantField.Value.Split('\n')
            .Where(tag => tag.Trim().StartsWith("<"))
            .Where(tag => tag != user.Mention)
            .ToArray();
        var canTags = canField.Value.Split('\n')
            .Where(tag => tag.Trim().StartsWith("<"))
            .Where(tag => tag != user.Mention)
            .ToList();
        var subTags = subField.Value.Split("\n")
            .Where(tag => tag.Trim().StartsWith("<"))
            .Where(tag => tag != user.Mention)
            .ToList();

        unsureTags.Add(user.Mention);
        if (relevantEvent.RoleId.HasValue)
        {
            try
            {
                var role = guild.GetRole(relevantEvent.RoleId.Value);
                await user.RemoveRoleAsync(role);
            }
            catch (Exception e)
            {
                // ROLE NOT FOUND
            }
        }

        var newEmbed = await RebuildEmbedAsync(canTags, unsureTags, cantTags, subTags, embed, relevantEvent);
        await message.ModifyAsync(msg => msg.Embed = newEmbed);
    }

    private async Task RemoveUserFromEventAsync(ulong userId, DataAccess.Contract.Event.Event relevantEvent,
        ulong messageId, ulong channelId)
    {
        var guild = _client.GetGuild(relevantEvent.GuildId);
        var user = guild.GetUser(userId);
        var message = (RestUserMessage)await guild
            .GetTextChannel(channelId)
            .GetMessageAsync(messageId);
        var embed = message.Embeds.Single();

        var canField = embed.Fields.Single(f => f.Name.Contains('✅'));
        var unsureField = embed.Fields.Single(f => f.Name.Contains('❓'));
        var cantField = embed.Fields.Single(f => f.Name.Contains('❌'));
        var subField = embed.Fields.SingleOrDefault(f => f.Name.Contains("🔼"));
        if (cantField.Value.Contains(user.Mention))
        {
            return;
        }

        var unsureTags = unsureField.Value.Split('\n')
            .Where(tag => tag.Trim().StartsWith("<"))
            .Where(tag => tag != user.Mention).ToList();
        var cantTags = cantField.Value.Split('\n')
            .Where(tag => tag.Trim().StartsWith("<"))
            .Where(tag => tag != user.Mention)
            .ToList();
        var canTags = canField.Value.Split('\n')
            .Where(tag => tag.Trim().StartsWith("<"))
            .Where(tag => tag != user.Mention)
            .ToList();
        var subTags = subField.Value.Split("\n")
            .Where(tag => tag.Trim().StartsWith("<"))
            .Where(tag => tag != user.Mention)
            .ToList();

        cantTags.Add(user.Mention);
        if (relevantEvent.RoleId.HasValue)
        {
            try
            {
                var role = guild.GetRole(relevantEvent.RoleId.Value);
                await user.RemoveRoleAsync(role);
            }
            catch (Exception e)
            {
                // ROLE NOT FOUND
            }
        }

        var newEmbed = await RebuildEmbedAsync(canTags, unsureTags, cantTags, subTags, embed, relevantEvent);
        await message.ModifyAsync(msg => msg.Embed = newEmbed);
    }

    private async Task AddUserToEventAsync(ulong userId, DataAccess.Contract.Event.Event relevantEvent, ulong messageId,
        ulong channelId)
    {
        var guild = _client.GetGuild(relevantEvent.GuildId);
        var user = guild.GetUser(userId);
        var message = (RestUserMessage)await guild
            .GetTextChannel(channelId)
            .GetMessageAsync(messageId);
        var embed = message.Embeds.Single();

        var canField = embed.Fields.Single(f => f.Name.Contains('✅'));
        var unsureField = embed.Fields.Single(f => f.Name.Contains('❓'));
        var cantField = embed.Fields.Single(f => f.Name.Contains('❌'));
        var subField = embed.Fields.SingleOrDefault(f => f.Name.Contains("🔼"));
        if (canField.Value.Contains(user.Mention))
        {
            return;
        }

        var unsureTags = unsureField.Value.Split('\n')
            .Where(tag => tag.Trim().StartsWith("<"))
            .Where(tag => tag != user.Mention).ToArray();
        var cantTags = cantField.Value.Split('\n')
            .Where(tag => tag.Trim().StartsWith("<"))
            .Where(tag => tag != user.Mention)
            .ToArray();
        var canTags = canField.Value.Split('\n')
            .Where(tag => tag.Trim().StartsWith("<"))
            .Where(tag => tag != user.Mention)
            .ToList();
        var subTags = subField.Value.Split("\n")
            .Where(tag => tag.Trim().StartsWith("<"))
            .Where(tag => tag != user.Mention)
            .ToList();
        if (!relevantEvent.MaxUsers.HasValue || canTags.Count < relevantEvent.MaxUsers.Value)
        {
            canTags.Add(user.Mention);
            if (relevantEvent.RoleId.HasValue)
            {
                try
                {
                    var role = guild.GetRole(relevantEvent.RoleId.Value);
                    await user.AddRoleAsync(role);
                }
                catch (Exception e)
                {
                    // ROLE NOT FOUND
                }
            }
        }
        else
        {
            subTags.Add(user.Mention);
        }

        var newEmbed = await RebuildEmbedAsync(canTags, unsureTags, cantTags, subTags, embed, relevantEvent);
        await message.ModifyAsync(msg => msg.Embed = newEmbed);
    }

    private async Task<Embed> RebuildEmbedAsync(IReadOnlyCollection<string> acceptedTages,
        IReadOnlyCollection<string> unsureTags,
        IReadOnlyCollection<string> cantTags,
        IReadOnlyCollection<string> subTags,
        IEmbed originalEmbed, DataAccess.Contract.Event.Event reactedEvent)
    {
        var language = await _dataAccess.GetUserLanguageAsync(reactedEvent.OwnerUserId);
        var description = reactedEvent.AutoDeleteDate.ToString("dd.MM HH:mm");
        if (reactedEvent.MaxUsers.HasValue)
        {
            description += $" (+{reactedEvent.MaxUsers.Value - acceptedTages.Count})";
        }

        var builder = new EmbedBuilder()
            .WithTitle(originalEmbed.Title)
            .WithAuthor(originalEmbed.Author.Value.Name)
            .WithColor(Color.Blue)
            .WithTimestamp(originalEmbed.Timestamp.Value)
            .WithDescription(description)
            .AddField(string.Format(Localize(nameof(EventResources.Field_Can), language), acceptedTages.Count),
                !acceptedTages.Any() ? "-" : string.Join("\n", acceptedTages))
            .AddField(string.Format(Localize(nameof(EventResources.Field_Cant), language), cantTags.Count),
                !cantTags.Any() ? "-" : string.Join("\n", cantTags))
            .AddField(string.Format(Localize(nameof(EventResources.Field_Unsure), language), unsureTags.Count),
                !unsureTags.Any() ? "-" : string.Join("\n", unsureTags))
            .AddField(string.Format(Localize(nameof(EventResources.Field_Sub), language), subTags.Count),
                string.Join("\n", subTags));

        return builder.Build();
    }

    private string Localize(string ressource, string language)
    {
        var ressourceType = typeof(EventResources);
        language = language == "ch" ? "de-ch" : language;
        lock (ressourceType)
        {
            var cultureProperty = ressourceType.GetProperty("Culture", BindingFlags.NonPublic | BindingFlags.Static);
            cultureProperty?.SetValue(null, new CultureInfo(language));
            return ressourceType.GetProperty(ressource, BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null)
                ?.ToString();
        }
    }


    public string ButtonEventPrefix => "event";

    public async Task ButtonPressedAsync(ulong userId, ulong messageId, ulong channelId, string customRewardId)
    {
        var parts = customRewardId.Split('_');
        var eventId = long.Parse(parts[1]);
        var action = parts[2];

        var relevantEvent = await _domain.GetEventByIdAsync(eventId);

        switch (action)
        {
            case "can":
                await AddUserToEventAsync(userId, relevantEvent, messageId, channelId);
                break;
            case "cant":
                await RemoveUserFromEventAsync(userId, relevantEvent, messageId, channelId);
                break;
            case "unsure":
                await UserUnsureAsync(userId, relevantEvent, messageId, channelId);
                break;
            case "sub":
                await UserSubAsync(userId, relevantEvent, messageId, channelId);
                break;
            default:
                return;
        }
    }

    private async Task UserSubAsync(ulong userId, DataAccess.Contract.Event.Event relevantEvent, ulong messageId,
        ulong channelId)
    {
        var guild = _client.GetGuild(relevantEvent.GuildId);
        var user = guild.GetUser(userId);
        var message = (RestUserMessage)await guild
            .GetTextChannel(channelId)
            .GetMessageAsync(messageId);
        var embed = message.Embeds.Single();

        var canField = embed.Fields.Single(f => f.Name.Contains('✅'));
        var unsureField = embed.Fields.Single(f => f.Name.Contains('❓'));
        var cantField = embed.Fields.Single(f => f.Name.Contains('❌'));
        var subField = embed.Fields.SingleOrDefault(f => f.Name.Contains("🔼"));
        if (subField.Value.Contains(user.Mention))
        {
            return;
        }

        var unsureTags = unsureField.Value.Split('\n')
            .Where(tag => tag.Trim().StartsWith("<"))
            .Where(tag => tag != user.Mention).ToArray();
        var cantTags = cantField.Value.Split('\n')
            .Where(tag => tag.Trim().StartsWith("<"))
            .Where(tag => tag != user.Mention)
            .ToArray();
        var canTags = canField.Value.Split('\n')
            .Where(tag => tag.Trim().StartsWith("<"))
            .Where(tag => tag != user.Mention)
            .ToList();
        var subTags = subField.Value.Split("\n")
            .Where(tag => tag.Trim().StartsWith("<"))
            .Where(tag => tag != user.Mention)
            .ToList();
        subTags.Add(user.Mention);

        var newEmbed = await RebuildEmbedAsync(canTags, unsureTags, cantTags, subTags, embed, relevantEvent);
        await message.ModifyAsync(msg => msg.Embed = newEmbed);
    }
}