using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Impostor.Server.Net;

namespace Impostor.Server
{
    public interface IGame
    {
        GameCode Code { get; }
        
        IEnumerable<IClientPlayer> Players { get; }
        
        IClientPlayer Host { get; }
        
        bool IsPublic { get; }
        IDictionary<object,object> Items { get; }

        IGameMessageWriter CreateMessage(MessageType type);

        bool TryGetPlayer(int id, [NotNullWhen(true)] out IClientPlayer player);
    }
}