using System;
using System.Linq;
using Hazel;
using Impostor.Server.Data;
using Impostor.Server.Exceptions;
using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Net.State
{
    internal partial class Game
    {
        public void HandleStartGame(MessageReader message)
        {
            GameState = GameStates.Started;
            
            using (var packet = MessageWriter.Get(SendOption.Reliable))
            {
                packet.CopyFrom(message);
                SendToAllExcept(packet, null);
            }
        }

        public void HandleJoinGame(ClientPlayer sender)
        {
            // Check if the IP of the player is banned.
            if (_bannedIps.Contains(sender.Client.Connection.EndPoint.Address))
            {
                sender.SendDisconnectReason(DisconnectReason.Banned);
                return;
            }
            
            // Check if;
            // - The player is already in this game.
            // - The game is full.
            if (sender.Game != this && _players.Count >= Options.MaxPlayers)
            {
                sender.SendDisconnectReason(DisconnectReason.GameFull);
                return;
            }
            
            // Check current player state.
            if (sender.Limbo == LimboStates.NotLimbo)
            {
                sender.SendDisconnectReason(DisconnectReason.Custom, "Invalid limbo state while joining.");
                return;
            }
            
            switch (GameState)
            {
                case GameStates.NotStarted:
                    HandleJoinGameNew(sender);
                    break;
                case GameStates.Ended:
                    HandleJoinGameNext(sender);
                    break;
                case GameStates.Started:
                    sender.SendDisconnectReason(DisconnectReason.GameStarted);
                    return;
                case GameStates.Destroyed:
                    sender.SendDisconnectReason(DisconnectReason.Custom, DisconnectMessages.Destroyed);
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void HandleEndGame(MessageReader message)
        {
            GameState = GameStates.Ended;
            
            // Broadcast end of the game.
            using (var packet = MessageWriter.Get(SendOption.Reliable))
            {
                packet.CopyFrom(message);
                SendToAllExcept(packet, null);
            }
            
            // Put all players in the correct limbo state.
            foreach (var player in _players)
            {
                player.Value.Limbo = LimboStates.PreSpawn;
            }
        }

        public void HandleAlterGame(MessageReader message, ClientPlayer sender, bool isPublic)
        {
            IsPublic = isPublic;
            
            using (var packet = MessageWriter.Get(SendOption.Reliable))
            {
                packet.CopyFrom(message);
                SendToAllExcept(packet, sender.Client.Id);
            }
        }
        
        public void HandleRemovePlayer(int playerId, DisconnectReason reason)
        {
            PlayerRemove(playerId, out _);

            // It's possible that the last player was removed, so check if the game is still around.
            if (GameState == GameStates.Destroyed)
            {
                return;
            }
            
            using (var packet = MessageWriter.Get(SendOption.Reliable))
            {
                WriteRemovePlayerMessage(packet, false, playerId, reason);
                SendToAllExcept(packet, playerId);
            }
        }

        public void HandleKickPlayer(int playerId, bool isBan)
        {
            Logger.Information("{0} - Player {1} has left.", CodeStr, playerId);
            
            using (var message = MessageWriter.Get(SendOption.Reliable))
            {
                // Send message to everyone that this player was kicked.
                WriteKickPlayerMessage(message, false, playerId, isBan);
                SendToAllExcept(message, null);
                
                if (PlayerRemove(playerId, out var player) && isBan)
                {
                    _bannedIps.Add(player.Client.Connection.EndPoint.Address);
                }
                
                // Rmeove the player from everyone's game.
                WriteRemovePlayerMessage(message, true, playerId, isBan 
                    ? DisconnectReason.Banned 
                    : DisconnectReason.Kicked);
                SendToAllExcept(message, player?.Client.Id);
            }
        }
        
        private void HandleJoinGameNew(ClientPlayer sender)
        {
            Logger.Information("{0} - Player {1} ({2}) is joining.", CodeStr, sender.Client.Name, sender.Client.Id);
            
            // Add player to the game.
            if (sender.Game == null)
            {
                PlayerAdd(sender);
            }

            using (var message = MessageWriter.Get(SendOption.Reliable))
            {
                WriteJoinedGameMessage(message, false, sender);
                WriteAlterGameMessage(message, false);
                
                sender.Limbo = LimboStates.NotLimbo;
                sender.Client.Send(message);

                BroadcastJoinMessage(message, true, sender);
            }
        }

        private void HandleJoinGameNext(ClientPlayer sender)
        {
            Logger.Information("{0} - Player {1} ({2}) is rejoining.", CodeStr, sender.Client.Name, sender.Client.Id);
            
            // Add player to the game.
            if (sender.Game == null)
            {
                PlayerAdd(sender);
            }
            
            // Check if the host joined and let everyone join.
            if (sender.Client.Id == HostId)
            {
                GameState = GameStates.NotStarted;
                
                // Spawn the host.
                HandleJoinGameNew(sender);
                
                // Pull players out of limbo.
                CheckLimboPlayers();
                return;
            }

            sender.Limbo = LimboStates.WaitingForHost;

            using (var packet = MessageWriter.Get(SendOption.Reliable))
            {
                WriteWaitForHostMessage(packet, false, sender);
                sender.Client.Send(packet);

                BroadcastJoinMessage(packet, true, sender);
            }
        }
    }
}