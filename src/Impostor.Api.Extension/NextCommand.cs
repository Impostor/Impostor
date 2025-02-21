namespace Impostor.Api.Extension;

public class NextCommand(string command, Func<CommandEventArgs, Task> onInvoke) : ISingleCommand
{
    private Func<CommandEventArgs, Task> OnInvoke { get; } = onInvoke;
    public string Command { get; } = command;

    public Task InvokeAsync(CommandEventArgs args)
    {
        return OnInvoke(args);
    }
}
