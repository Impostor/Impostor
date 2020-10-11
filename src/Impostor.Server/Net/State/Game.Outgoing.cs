using System.Linq;
using Impostor.Server.Net.Messages;
using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Net.State
{
    internal partial class Game
    {
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