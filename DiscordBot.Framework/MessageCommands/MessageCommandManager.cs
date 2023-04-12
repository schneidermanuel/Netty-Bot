using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Framework.Contract.Boot;
using DiscordBot.Framework.Contract.Modularity;
using DiscordBot.Framework.Contract.Modularity.Commands;
using DiscordBot.Framework.Extentions;

namespace DiscordBot.Framework.MessageCommands;

internal class MessageCommandManager : IBootStep
{
    private readonly DiscordSocketClient _client;
    private readonly IEnumerable<ICommandModule> _commandModules;
    private readonly IDictionary<string, CommandInfo> _messageCommands;

    public MessageCommandManager(DiscordSocketClient client, IEnumerable<ICommandModule> commandModules)
    {
        _client = client;
        _commandModules = commandModules;
        _messageCommands = new Dictionary<string, CommandInfo>();
    }

    private async Task MessageCommandExecuted(SocketMessageCommand arg)
    {
        var name = arg.CommandName;
        if (!_messageCommands.ContainsKey(name))
        {
            Console.WriteLine($"Message Command '{name}' not found!");
            return;
        }

        var command = _messageCommands[name];
        command.MethodInfo.Invoke(command.CommandModule, new object[] { arg });
    }

    public async Task BootAsync()
    {
        foreach (var module in _commandModules)
        {
            var commands = module.BuildMessageCommandInfos();
            _messageCommands.AddRange(
                commands.ToDictionary(
                    cmd => cmd.Key,
                    cmd => new CommandInfo
                    {
                        MethodInfo = cmd.Value, CommandModule = module
                    }));
        }

        _client.MessageCommandExecuted += MessageCommandExecuted;
        await Task.CompletedTask;
    }

    public BootOrder StepPosition => BootOrder.End;
}