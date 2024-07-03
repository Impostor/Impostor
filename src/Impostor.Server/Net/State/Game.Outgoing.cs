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
            var writer = StartGameData(targetClientId, type);

            writer.StartMessage((byte)GameDataTag.RpcFlag);
            writer.WritePacked(targetNetId);
            writer.Write((byte)callId);

            return writer;
        }

        public ValueTask FinishRpcAsync(IMessageWriter writer, int? targetClientId = null)
        {
            writer.EndMessage();
            return FinishGameDataAsync(writer, targetClientId);
        }

        /// <summary>Start a GameData(To) message.</summary>
        /// <param name="targetClientId">The client to target if needed, `null` otherwise.</param>
        /// <param name="type">The type of message to send, defaults to reliable.</param>
        /// <returns>MessageWriter that should be handed back to <see cref="FinishGameDataAsync"/>.</returns>
        private IMessageWriter StartGameData(int? targetClientId = null, MessageType type = MessageType.Reliable)
        {
            var writer = MessageWriter.Get(type);

            if (targetClientId == null || targetClientId < 0)
            {
                writer.StartMessage(MessageFlags.GameData);
                Code.Serialize(writer);
            }
            else
            {
                writer.StartMessage(MessageFlags.GameDataTo);
                Code.Serialize(writer);
                writer.WritePacked(targetClientId.Value);
            }

            return writer;
        }

        /// <summary>Finalize and send a GameData packet.</summary>
        /// <param name="writer">MessageWriter received from <see cref="StartGameData"/>.</param>
        /// <param name="targetClientId">Same target ClientId passed to StartGameData.</param>
        /// <returns>Task that sends the packet.</returns>
        private ValueTask FinishGameDataAsync(IMessageWriter writer, int? targetClientId = null)
        {
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
            var players = _players
                .Where(x => x.Value != player)
                .Select(x => x.Value)
                .ToArray();

            Message07JoinedGameS2C.Serialize(message, clear, Code, player.Client.Id, HostId, players);
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

        private ValueTask SendObjectSpawn(InnerNetObject obj, int? targetClientId = null)
        {
            using var writer = StartGameData(targetClientId);
            writer.StartMessage((byte)GameDataTag.SpawnFlag);
            writer.WritePacked(11u);        // TODO don't hardcode
            writer.WritePacked(obj.OwnerId);
            writer.Write((byte)obj.SpawnFlags);

            var components = obj.GetComponentsInChildren<InnerNetObject>();
            writer.WritePacked(components.Count);
            foreach (var comp in components)
            {
                writer.WritePacked(obj.NetId);
                writer.StartMessage(1);
                comp.SerializeAsync(writer, true);
                writer.EndMessage();
            }

            writer.EndMessage();
            return FinishGameDataAsync(writer, targetClientId);
        }

        private ValueTask SendObjectDespawn(InnerNetObject obj, int? targetClientId = null)
        {
            using var writer = StartGameData(targetClientId);
            writer.StartMessage((byte)GameDataTag.DespawnFlag);
            writer.WritePacked(obj.NetId);
            writer.EndMessage();
            return FinishGameDataAsync(writer, targetClientId);
        }

        private async ValueTask SendObjectData(InnerNetObject obj, int? targetClientId = null)
        {
            using var writer = StartGameData(targetClientId);
            writer.StartMessage((byte)GameDataTag.DataFlag);
            writer.WritePacked(obj.NetId);
            await obj.SerializeAsync(writer, false);
            writer.EndMessage();
            await FinishGameDataAsync(writer, targetClientId);
        }
    }
}
