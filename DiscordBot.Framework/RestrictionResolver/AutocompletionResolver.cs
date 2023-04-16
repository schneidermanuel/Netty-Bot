using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Framework.Contract.Boot;

namespace DiscordBot.Framework.RestrictionResolver;

internal class AutocompletionResolver : IBootStep
{
    private readonly DiscordSocketClient _client;
    private readonly IReadOnlyCollection<IRestrictionResolver> _resolvers;

    public AutocompletionResolver(DiscordSocketClient client, IEnumerable<IRestrictionResolver> resolvers)
    {
        _client = client;
        _resolvers = resolvers.ToArray();
    }

    public Task BootAsync()
    {
        _client.AutocompleteExecuted += AutocompleteRequired;
        return Task.CompletedTask;
    }

    private async Task AutocompleteRequired(SocketAutocompleteInteraction interaction)
    {
        var commandName = interaction.Data.CommandName;
        var parameterName = interaction.Data.Current.Name;
        var currentText = (interaction.Data.Current.Value.ToString() ?? string.Empty).ToLower();

        var resolver = _resolvers.SingleOrDefault(r => r.IsResponsible(commandName, parameterName));
        if (resolver != null)
        {
            var items = resolver.PermittedValues;
            var filteredStage1 = items.Where(item => item.Key.ToLower().StartsWith(currentText)).ToArray();
            var filteredStage2 = items.Where(item => item.Key.ToLower().Contains(currentText)).ToArray();
            var filteredStage3 = items.Where(item => item.Value.ToLower().StartsWith(currentText)).ToArray();
            var filteredStage4 = items.Where(item => item.Value.ToLower().Contains(currentText)).ToArray();

            var autocompletes = filteredStage1
                .Union(filteredStage2)
                .Union(filteredStage3)
                .Union(filteredStage4)
                .Distinct()
                .Take(25)
                .Select(x => new AutocompleteResult($"{x.Key} ({x.Value})", x.Key))
                .ToArray();
            await interaction.RespondAsync(autocompletes);
        }
    }

    public BootOrder StepPosition => BootOrder.Async;
}