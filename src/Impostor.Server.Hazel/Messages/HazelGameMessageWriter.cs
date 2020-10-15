using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hazel;
using Impostor.Server.Games;
using Impostor.Server.Net;
using Impostor.Server.Net.Messages;

namespace Impostor.Server.Hazel.Messages
{
    internal class HazelGameMessageWriter : HazelMessageWriter, IGameMessageWriter
    {
        private readonly IGame _game;

        public HazelGameMessageWriter(MessageType type, IGame game)
            : base(type)
        {
            _game = game;
        }

        private IEnumerable<Connection> GetConnections(Func<IClientPlayer, bool> filter)
        {
            return _game.Players
                .Where(filter)
                .Select(p => p.Client.Connection)
                .OfType<HazelConnection>()
                .Select(c => c.InnerConnection);
        }

        public ValueTask SendToAllAsync(LimboStates states)
        {
            foreach (var connection in GetConnections(x => x.Limbo.HasFlag(states)))
            {
                connection.Send(Writer);
            }

            return default;
        }

        public ValueTask SendToAllExceptAsync(int senderId, LimboStates states)
        {
            foreach (var connection in GetConnections(x =>
                x.Limbo.HasFlag(states) &&
                x.Client.Id != senderId))
            {
                connection.Send(Writer);
            }
            return default;
        }

        public ValueTask SendToAsync(int id)
        {
            if (_game.TryGetPlayer(id, out var player)
                && player.Client.Connection is HazelConnection hazelConnection)
            {
                hazelConnection.InnerConnection.Send(Writer);
            }

            return default;
        }
    }
}