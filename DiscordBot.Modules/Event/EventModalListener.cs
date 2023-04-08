using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.Event;

internal class EventModalListener : IModalListener
{
    private readonly DiscordSocketClient _client;

    public EventModalListener(DiscordSocketClient client)
    {
        _client = client;
    }

    public string ButtonEventPrefix => "eventLineup";

    public async Task SubmittedAsync(ulong userId, SocketModal modal)
    {
        var parts = modal.Data.CustomId.Split('_');
        var guildId = ulong.Parse(parts[1]);
        var channelId = ulong.Parse(parts[2]);
        var messageId = ulong.Parse(parts[3]);

        var message = await
            ((ISocketMessageChannel)
                _client.GetGuild(guildId)
                    .GetChannel(channelId))
            .GetMessageAsync(messageId);

        var embed = message.Embeds.Single();

        var enemy = modal.Data.Components.Single(component => component.CustomId == "event_enemy").Value;
        var fc = modal.Data.Components.Single(component => component.CustomId == "event_fc").Value;

        var cans = embed.Fields.Single(field => field.Name.Contains('✅'));
        var subs = embed.Fields.Single(field => field.Name.Contains("🔼"));
        var title = embed.Title;
        var desc = embed.Description;

        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine(title);
        stringBuilder.AppendLine(desc);
        stringBuilder.AppendLine();
        if (!string.IsNullOrEmpty(enemy))
        {
            stringBuilder.AppendLine($"VS: {enemy}");
            stringBuilder.AppendLine();
        }

        stringBuilder.AppendLine(cans.Name);
        stringBuilder.AppendLine(cans.Value);
        stringBuilder.AppendLine();
        stringBuilder.AppendLine(subs.Name);
        stringBuilder.AppendLine(subs.Value);
        stringBuilder.AppendLine();
        if (!string.IsNullOrEmpty(fc))
        {
            stringBuilder.AppendLine($"Host: {fc}");
        }

        await modal.RespondAsync(stringBuilder.ToString());
    }
}