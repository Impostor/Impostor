using System.Linq;
using System.Threading.Tasks;
using Impostor.Api.Innersloth.Data;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages;
using Impostor.Server.Net.Hazel;
using Impostor.Server.Net.Messages;

namespace Impostor.Server.Net.State
{
    internal partial class Game
    {
        public ValueTask SendToAllAsync(IMessageWriter writer, LimboStates states = LimboStates.NotLimbo)
        {
            foreach (var connection in GetConnections(x => x.Limbo.HasFlag(states)))
            {
                connection.Send(writer);
            }

            return default;
        }

        public ValueTask SendToAllExceptAsync(IMessageWriter writer, int senderId, LimboStates states = LimboStates.NotLimbo)
        {
            foreach (var connection in GetConnections(x =>
                x.Limbo.HasFlag(states) &&
                x.Client.Id != senderId))
            {
                connection.Send(writer);
            }

            return default;
        }

        public ValueTask SendToAsync(IMessageWriter writer, int id)
        {
            if (TryGetPlayer(id, out var player) && player.Client.Connection is HazelConnection hazelConnection)
            {
                hazelConnection.InnerConnection.Send(writer);
            }

            return default;
        }

        private void WriteRemovePlayerMessage(IMessageWriter message, bool clear, int playerId, DisconnectReason reason)
        {
            Message04RemovePlayer.Serialize(message, clear, Code, playerId, HostId, reason);
        }

        private void WriteJoinedGameMessage(IMessageWriter message, bool clear, IClientPlayer player)
        {
            var playerIds = _players
                .Where(x => x.Value != player)
                .Select(x => x.Key)
                .ToArray();

            Message07JoinedGame.Serialize(message, clear, Code, player.Client.Id, HostId, playerIds);
        }

        private void WriteAlterGameMessage(IMessageWriter message, bool clear, bool isPublic)
        {
            Message10AlterGame.Serialize(message, clear, Code, isPublic);
        }

        private void WriteKickPlayerMessage(IMessageWriter message, bool clear, int playerId, bool isBan)
        {
            Message11KickPlayer.Serialize(message, clear, Code, playerId, isBan);
        }

        private void WriteWaitForHostMessage(IMessageWriter message, bool clear, IClientPlayer player)
        {
            Message12WaitForHost.Serialize(message, clear, Code, player.Client.Id);
        }
    }
}