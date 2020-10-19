using System.Collections.Generic;
using System.Net;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.Data;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages;

namespace Impostor.Api.Games
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