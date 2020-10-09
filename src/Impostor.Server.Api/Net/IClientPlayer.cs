using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Net
{
    public interface IClientPlayer
    {
        IClient Client { get; }
        
        IGame Game { get; }
        
        LimboStates Limbo { get; }
    }
}