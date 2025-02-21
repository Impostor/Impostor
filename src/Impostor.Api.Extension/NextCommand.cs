namespace Impostor.Api.Extension;

public class NextCommand(string command, Func<CommandEventArgs, Task> onInvoke) : ISingleCommand
{
    public string Command { get; } = command;
    private Func<CommandEventArgs, Task> OnInvoke { get; set; } = onInvoke;
    
    public Task InvokeAsync(CommandEventArgs args) => OnInvoke(args);
}
