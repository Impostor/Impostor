using System;
using System.Linq;
using Hazel;
using Impostor.Server.Data;
using Impostor.Server.Exceptions;
using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Net.State
{
    public partial class Game
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
            if (_bannedIps.Contains(sender.Client.Connection.EndPoint.Address))
            {
                sender.SendDisconnectReason(DisconnectReason.Banned);
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
            
            // Remove all players from this game.
            foreach (var player in _players)
            {
                player.Value.Game = null;
            }
            
            _players.Clear();
        }

        public void HandleAlterGame(MessageReader message, ClientPlayer sender, bool isPublic)
        {
            IsPublic = isPublic;
            
            using (var packet = MessageWriter.Get(SendOption.Reliable))
            {
                packet.CopyFrom(message);
                SendToAllExcept(packet, sender);
            }
        }
        
        public void HandleRemovePlayer(int playerId, DisconnectReason reason)
        {
            if (_players.TryRemove(playerId, out var player))
            {
                player.Game = null;
            }
            
            Logger.Information("{0} - Player {1} ({2}) has left.", CodeStr, player?.Client.Name, playerId);

            // Game is empty, remove it.
            if (_players.Count == 0)
            {
                GameState = GameStates.Destroyed;

                // Remove instance reference.
                _gameManager.Remove(Code);
                return;
            }

            // Host migration.
            if (HostId == playerId)
            {
                var newHost = _players.First().Value;
                HostId = newHost.Client.Id;
                Logger.Information("{0} - Assigned {1} ({2}) as new host.", CodeStr, newHost.Client.Name, newHost.Client.Id);
            }

            using (var packet = MessageWriter.Get(SendOption.Reliable))
            {
                WriteRemovePlayerMessage(packet, false, playerId, reason);
                SendToAllExcept(packet, player);
            }
        }

        public void HandleKickPlayer(int playerId, bool isBan)
        {
            _players.TryGetValue(playerId, out var p);
            Logger.Information("{0} - Player {1} ({2}) has left.", CodeStr, p?.Client.Name, playerId);
            
            using (var message = MessageWriter.Get(SendOption.Reliable))
            {
                WriteKickPlayerMessage(message, false, playerId, isBan);
                SendToAllExcept(message, null);
                
                if (_players.TryRemove(playerId, out var player))
                {
                    player.Game = null;

                    if (isBan)
                    {
                        _bannedIps.Add(player.Client.Connection.EndPoint.Address);
                    }
                }
                
                WriteRemovePlayerMessage(message, true, playerId, isBan 
                    ? DisconnectReason.Banned 
                    : DisconnectReason.Kicked);
                SendToAllExcept(message, player);
            }
        }
        
        private void HandleJoinGameNew(ClientPlayer sender)
        {
            Logger.Information("{0} - Player {1} ({2}) is joining.", CodeStr, sender.Client.Name, sender.Client.Id);
            
            // Store player.
            if (!_players.TryAdd(sender.Client.Id, sender))
            {
                throw new AmongUsException("Failed to add player to game.");
            }
            
            // Assign player to this game for future packets.
            sender.Game = this;

            // Assign hostId if none is set.
            if (HostId == -1)
            {
                HostId = sender.Client.Id;
            }

            using (var message = MessageWriter.Get(SendOption.Reliable))
            {
                WriteJoinedGameMessage(message, false, sender);
                WriteAlterGameMessage(message, false);
                
                sender.Client.Send(message);

                BroadcastJoinMessage(message, true, sender);
            }
        }

        private void HandleJoinGameNext(ClientPlayer sender)
        {
            Logger.Information("{0} - Player {1} ({2}) is rejoining.", CodeStr, sender.Client.Name, sender.Client.Id);
            
            if (sender.Client.Id == HostId)
            {
                GameState = GameStates.NotStarted;
                HandleJoinGameNew(sender);

                using (var message = MessageWriter.Get(SendOption.Reliable))
                {
                    foreach (var (_, player) in _players.Where(x => x.Value != sender))
                    {
                        WriteJoinedGameMessage(message, true, player);
                        WriteAlterGameMessage(message, false);
                        player.Client.Send(message);
                    }
                }
                
                return;
            }

            if (_players.Count >= 9)
            {
                sender.SendDisconnectReason(DisconnectReason.GameFull);
                return;
            }

            // Store player.
            if (!_players.TryAdd(sender.Client.Id, sender))
            {
                throw new AmongUsException("Failed to add player to game.");
            }
            
            // Assign player to this game for future packets.
            sender.Game = this;

            using (var packet = MessageWriter.Get(SendOption.Reliable))
            {
                WriteWaitForHostMessage(packet, false, sender);
                sender.Client.Send(packet);

                BroadcastJoinMessage(packet, true, sender);
            }
        }
    }
}