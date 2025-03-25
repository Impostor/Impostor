namespace Impostor.Api.Extension.Commands;

public interface ICommand;

public interface ISystemCommand : ICommand
{
    public ValueTask<bool> InvokeAsync(string command, string[] argsArray);
}

public interface ISingleCommand : ICommand
{
    public string Command { get; }
    public Task InvokeAsync(CommandEventArgs args);
}
