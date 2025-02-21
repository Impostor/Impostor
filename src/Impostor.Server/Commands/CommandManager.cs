using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Impostor.Api.Extension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Commands;

public sealed class CommandManager(ConsoleCommandService commandService, IServiceProvider provider,ILogger<CommandManager> logger) : ICommandManager
{
    public IReadOnlyList<ICommand> Commands => _allCommands.AsReadOnly();
    
    private readonly Dictionary<string,ISingleCommand> _singleCommands = [];
    private readonly List<ISystemCommand> _systemCommands = [];
    private readonly List<ICommand> _allCommands = [];
    
    internal async Task HandleCommandAsync(string commandString)
    {
        var args = commandString.Split(" ");
        var command = args[0];

        var argArray = args.Skip(1).ToArray();
        if (await commandService.HandleDefaultCommandAsync(command, argArray))
            return;

        foreach (var sc in _systemCommands)
        {
            if (await sc.InvokeAsync(command, argArray))
                return;
        }

        var eventArg = new CommandEventArgs(this, argArray);
        if (_singleCommands.TryGetValue(command, out var singleCommand))
        {
            await singleCommand.InvokeAsync(eventArg);
        }
    }

    public ICommandManager RegisterCommand(ICommand command)
    {
        switch (command)
        {
            case ISingleCommand singleCommand:
                _singleCommands[singleCommand.Command] = singleCommand;
                break;
            case ISystemCommand systemCommand:
                _systemCommands.Add(systemCommand);
                break;
        }

        _allCommands.Add(command);
        return this;
    }

    public ICommandManager RegisterCommand<T>() where T : ICommand
    {
        return RegisterCommand(ActivatorUtilities.CreateInstance<T>(ServiceProvider));
    }

    public IServiceProvider ServiceProvider { get; } = provider;
}
