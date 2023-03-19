using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.GeburtstagList;

namespace DiscordBot.Modules.BirthdayList;

public class BirthdayListManager
{
    private readonly IGeburtstagListDomain _domain;
    private readonly DiscordSocketClient _client;
    private List<Birthday> _birthdays;

    public BirthdayListManager(IGeburtstagListDomain domain, DiscordSocketClient client)
    {
        _domain = domain;
        _client = client;
    }

    public async Task RefreshSingleBirthdayChannel(BirthdayChannel channelSetup)
    {
        try
        {
            var guild = _client.GetGuild(channelSetup.GuildId);
            var channel = (SocketTextChannel)guild.GetChannel(channelSetup.ChannelId);
            var birthdaysOnServer = await GetBirthdaysFromServer(guild);

            var janMsg = (IUserMessage)await channel.GetMessageAsync(channelSetup.JanMessageId);
            var janMsgContent = BuildMessageContent("Januar", birthdaysOnServer, 1);
            await janMsg.ModifyAsync(x => x.Content = janMsgContent);
            var febMsg = (IUserMessage)await channel.GetMessageAsync(channelSetup.FebMessageId);
            var febMsgContent = BuildMessageContent("Februar", birthdaysOnServer, 2);
            await febMsg.ModifyAsync(x => x.Content = febMsgContent);
            var marMsg = (IUserMessage)await channel.GetMessageAsync(channelSetup.MarMessageId);
            var marMsgContent = BuildMessageContent("März", birthdaysOnServer, 3);
            await marMsg.ModifyAsync(x => x.Content = marMsgContent);
            var aprMsg = (IUserMessage)await channel.GetMessageAsync(channelSetup.AprMessageId);
            var aprMsgContent = BuildMessageContent("April", birthdaysOnServer, 4);
            await aprMsg.ModifyAsync(x => x.Content = aprMsgContent);
            var maiMsg = (IUserMessage)await channel.GetMessageAsync(channelSetup.MaiMessageId);
            var maiMsgContent = BuildMessageContent("Mai", birthdaysOnServer, 5);
            await maiMsg.ModifyAsync(x => x.Content = maiMsgContent);
            var junMsg = (IUserMessage)await channel.GetMessageAsync(channelSetup.JunMessageId);
            var junMsgContent = BuildMessageContent("Juni", birthdaysOnServer, 6);
            await junMsg.ModifyAsync(x => x.Content = junMsgContent);
            var julMsg = (IUserMessage)await channel.GetMessageAsync(channelSetup.JulMessageId);
            var julMsgContent = BuildMessageContent("Juli", birthdaysOnServer, 7);
            await julMsg.ModifyAsync(x => x.Content = julMsgContent);
            var augMsg = (IUserMessage)await channel.GetMessageAsync(channelSetup.AugMessageId);
            var augMsgContent = BuildMessageContent("August", birthdaysOnServer, 8);
            await augMsg.ModifyAsync(x => x.Content = augMsgContent);
            var sepMsg = (IUserMessage)await channel.GetMessageAsync(channelSetup.SepMessageId);
            var sepMsgContent = BuildMessageContent("September", birthdaysOnServer, 9);
            await sepMsg.ModifyAsync(x => x.Content = sepMsgContent);
            var octMsg = (IUserMessage)await channel.GetMessageAsync(channelSetup.OctMessageId);
            var octMsgContent = BuildMessageContent("Oktober", birthdaysOnServer, 10);
            await octMsg.ModifyAsync(x => x.Content = octMsgContent);
            var novMsg = (IUserMessage)await channel.GetMessageAsync(channelSetup.NovMessageId);
            var novMsgContent = BuildMessageContent("November", birthdaysOnServer, 11);
            await novMsg.ModifyAsync(x => x.Content = novMsgContent);
            var dezMsg = (IUserMessage)await channel.GetMessageAsync(channelSetup.DezMessageId);
            var dezMsgContent = BuildMessageContent("Dezember", birthdaysOnServer, 12);
            await dezMsg.ModifyAsync(x => x.Content = dezMsgContent);
        }
        catch (NullReferenceException)
        {
            Console.WriteLine(
                $"Geburtstagsaktualisierung für server {channelSetup.GuildId} nicht möglich, lösche aus DB");
            await _domain.DeleteBirthdayChannelAsync(channelSetup.Id);
        }
    }

    private string BuildMessageContent(string prefix, Dictionary<string, DateTime> birthdaysOnServer, int month)
    {
        var output = $"{prefix}:\n";

        return birthdaysOnServer
            .Where(x => x.Value.Month == month)
            .Aggregate(output, (current, birthday) =>
                current + $"{birthday.Key}: {birthday.Value.Day:00}.{birthday.Value.Month:00}.\n");
    }

     private async Task<Dictionary<string, DateTime>> GetBirthdaysFromServer(SocketGuild guild)
    {
        var output = new Dictionary<string, DateTime>();
        if (_birthdays == null)
        {
            _birthdays = await _domain.GetAllGeburtstageAsync();

        }
        foreach (var birthday in _birthdays.OrderBy(x => x.Geburtsdatum))
        {
            try
            {
                var user = guild.GetUser(birthday.UserId);
                var mention = user.Mention;
                output.Add(mention, birthday.Geburtsdatum);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        return output;
    }

    public async Task RefreshAllBirthdayChannel()
    {
        _birthdays = await _domain.GetAllGeburtstageAsync();
        var channels = await _domain.GetAllGeburtstagsChannelAsync();
        foreach (var channel in channels)
        {
            await RefreshSingleBirthdayChannel(channel);
        }
    }

    public async Task CheckNewBirthday()
    {
        var birthdays = _birthdays.Where(x =>
            x.Geburtsdatum.Day == DateTime.Now.Day && x.Geburtsdatum.Month == DateTime.Now.Month);
        var subbedChannel = (await _domain.GetAllSubbedChannelAsync()).ToList();
        foreach (var birthday in birthdays)
        {
            foreach (var subChannel in subbedChannel)
            {
                var guild = _client.GetGuild(subChannel.GuildId);
                var user = guild.GetUser(birthday.UserId);
                if (user == null)
                {
                    continue;
                }

                try
                {
                    var channel =
                        (SocketTextChannel)guild.GetChannel(subChannel.ChannelId);
                    var message = $"🍰HURRA🍰\n{user.Mention} hat heute Geburtstag!";
                    await channel.SendMessageAsync(message);
                    if (await _domain.HasGuildSetupBirthdayRoleAsync(guild.Id))
                    {
                        var roleId = await _domain.RetrieveBirthdayRoleIdForGuildAsync(guild.Id);
                        if (user.Roles.Any(role => role.Id == roleId))
                        {
                            continue;
                        }

                        var role = guild.GetRole(roleId);
                        await user.AddRoleAsync(role);
                        await _domain.InsertBirthdayRoleAssotiation(guild.Id, user.Id);
                        Console.WriteLine($"[BIRTHDAY ROLE] Added '{role.Name}' to '{user.Username}'");
                    }
                }
                catch (NullReferenceException)
                {
                    Console.WriteLine(
                        $"Cannot process Sub of {subChannel.GuildId}/{subChannel.ChannelId}, Delete from DB!");
                    await _domain.DeleteSubbedChannelAsync(subChannel.GuildId, subChannel.ChannelId);
                }
            }
        }
    }

    public async Task RemoveBirthdayRolesFromUsers()
    {
        var birthdayRoleAssociations = await _domain.RetrieveAllBirthdayRoleAssotiations();
        var birthdayRoleSetups = (await _domain.RetrieveAllBirthdayRoleSetupsAsync()).ToList();
        foreach (var birthdayRoleAssociation in birthdayRoleAssociations)
        {
            if (birthdayRoleSetups.All(setup => setup.GuildId != birthdayRoleAssociation.GuildId))
            {
                continue;
            }

            try
            {
                var guild = _client.GetGuild(birthdayRoleAssociation.GuildId);
                var user = guild.GetUser(birthdayRoleAssociation.UserId);
                var setup = birthdayRoleSetups.Single(setup => setup.GuildId == birthdayRoleAssociation.GuildId);
                var role = guild.GetRole(setup.RoleId);
                await user.RemoveRoleAsync(role);
                await _domain.DeleteAssociationAsync(birthdayRoleAssociation);
                Console.WriteLine($"[BIRTHDAY ROLE] - Removed '{role.Name}' from '{user.Username}'");
            }
            catch
            {
                // Ignored
            }
        }
    }
}