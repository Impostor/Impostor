namespace Impostor.Api.Extension;

public class CommandEventArgs(ICommandManager manager, string[] args) : EventArgs
{
    public ICommandManager Sender { get; set; } = manager;
    public string[] Args { get; set; } = args;
}
