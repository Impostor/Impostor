using System.Collections.Generic;
using System.Net;
using Impostor.Server.Net;
using Impostor.Server.Net.Messages;
using Impostor.Shared.Innersloth;
using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Games
{
    public interface IGame
    {
        GameOptionsData Options { get; }

        GameCode Code { get; }

        GameStates GameState { get; }

        IEnumerable<IClientPlayer> Players { get; }

        IPEndPoint PublicIp { get; }

        int PlayerCount { get; }

        IClientPlayer Host { get; }

        bool IsPublic { get; }

        IDictionary<object, object> Items { get; }

        int HostId { get; }

        IGameMessageWriter CreateMessage(MessageType type);
    }
}