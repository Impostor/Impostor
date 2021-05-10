using System.Linq;
using System.Threading.Tasks;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Messages;
using Impostor.Api.Net.Messages.S2C;
using Impostor.Hazel;
using Impostor.Server.Net.Inner;

namespace Impostor.Server.Net.State
{
    internal partial class Game
    {
        public async ValueTask SendToAllAsync(IMessageWriter writer, LimboStates states = LimboStates.NotLimbo)
        {
            foreach (var connection in GetConnections(x => x.Limbo.HasFlag(states)))
            {
                await connection.SendAsync(writer);
            }
        }

        public async ValueTask SendToAllExceptAsync(IMessageWriter writer, int senderId, LimboStates states = LimboStates.NotLimbo)
        {
            foreach (var connection in GetConnections(x =>
                x.Limbo.HasFlag(states) &&
                x.Client.Id != senderId))
            {
                await connection.SendAsync(writer);
            }
        }

        public async ValueTask SendToAsync(IMessageWriter writer, int id)
        {
            if (TryGetPlayer(id, out var player))
            {
                await player.Client.Connection.SendAsync(writer);
            }
        }

        public IMessageWriter StartRpc(uint targetNetId, RpcCalls callId, int? targetClientId = null, MessageType type = MessageType.Reliable)
        {
            var writer = MessageWriter.Get(type);

            if (targetClientId == null || targetClientId < 0)
            {
                writer.StartMessage(MessageFlags.GameData);
                writer.Write(Code);
            }
            else
            {
                writer.StartMessage(MessageFlags.GameDataTo);
                writer.Write(Code);
                writer.WritePacked(targetClientId.Value);
            }

            writer.StartMessage(GameDataTag.RpcFlag);
            writer.WritePacked(targetNetId);
            writer.Write((byte)callId);

            return writer;
        }

        public ValueTask FinishRpcAsync(IMessageWriter writer, int? targetClientId = null)
        {
            writer.EndMessage();
            writer.EndMessage();

            return targetClientId.HasValue
                ? SendToAsync(writer, targetClientId.Value)
                : SendToAllAsync(writer);
        }

        private void WriteRemovePlayerMessage(IMessageWriter message, bool clear, int playerId, DisconnectReason reason)
        {
            Message04RemovePlayerS2C.Serialize(message, clear, Code, playerId, HostId, reason);
        }

        private void WriteJoinedGameMessage(IMessageWriter message, bool clear, IClientPlayer player)
        {
            var playerIds = _players
                .Where(x => x.Value != player)
                .Select(x => x.Key)
                .ToArray();

            Message07JoinedGameS2C.Serialize(message, clear, Code, player.Client.Id, HostId, playerIds);
        }

        private void WriteAlterGameMessage(IMessageWriter message, bool clear, bool isPublic)
        {
            Message10AlterGameS2C.Serialize(message, clear, Code, isPublic);
        }

        private void WriteKickPlayerMessage(IMessageWriter message, bool clear, int playerId, bool isBan)
        {
            Message11KickPlayerS2C.Serialize(message, clear, Code, playerId, isBan);
        }

        private void WriteWaitForHostMessage(IMessageWriter message, bool clear, IClientPlayer player)
        {
            Message12WaitForHostS2C.Serialize(message, clear, Code, player.Client.Id);
        }
    }
}
