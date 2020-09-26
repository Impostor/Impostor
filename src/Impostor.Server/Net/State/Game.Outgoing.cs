using System.Linq;
using Hazel;
using Impostor.Server.Net.Messages;
using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Net.State
{
    internal partial class Game
    {
        private void WriteRemovePlayerMessage(MessageWriter message, bool clear, int playerId, DisconnectReason reason)
        {
            Message04RemovePlayer.Serialize(message, clear, Code, playerId, HostId, reason);
        }
        
        private void WriteJoinedGameMessage(MessageWriter message, bool clear, ClientPlayer player)
        {
            var playerIds = _players
                .Where(x => x.Value != player)
                .Select(x => x.Key)
                .ToArray();
            
            Message07JoinedGame.Serialize(message, clear, Code, player.Client.Id, HostId, playerIds);
        }

        private void WriteAlterGameMessage(MessageWriter message, bool clear)
        {
            Message10AlterGame.Serialize(message, clear, Code);
        }

        private void WriteKickPlayerMessage(MessageWriter message, bool clear, int playerId, bool isBan)
        {
            Message11KickPlayer.Serialize(message, clear, Code, playerId, isBan);
        }
        
        private void WriteWaitForHostMessage(MessageWriter message, bool clear, ClientPlayer player)
        {
            Message12WaitForHost.Serialize(message, clear, Code, player.Client.Id);
        }
    }
}