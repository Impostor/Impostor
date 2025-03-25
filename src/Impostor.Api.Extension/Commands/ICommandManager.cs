namespace Impostor.Api.Extension.Commands;

public interface ICommandManager
{
    public IServiceProvider ServiceProvider { get; }

    public ICommandManager RegisterCommand(ICommand command);

    public ICommandManager RegisterCommand<T>() where T : ICommand;

    public Task HandleCommandAsync(string commandString);

    public IReadOnlyList<ICommand> Commands { get; }
}
