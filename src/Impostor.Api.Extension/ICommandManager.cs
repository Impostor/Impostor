namespace Impostor.Api.Extension;

public interface ICommandManager
{
    public IServiceProvider ServiceProvider { get; }

    public ICommandManager RegisterCommand(ICommand command);

    public ICommandManager RegisterCommand<T>() where T : ICommand;
}
