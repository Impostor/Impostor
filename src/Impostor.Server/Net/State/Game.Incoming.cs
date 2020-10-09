using System;
using System.Threading.Tasks;
using Impostor.Server.Data;
using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Net.State
{
    internal partial class Game
    {
        public async ValueTask HandleStartGame(IMessageReader message)
        {
            GameState = GameStates.Started;

            using var packet = CreateMessage(MessageType.Reliable);
            message.CopyTo(packet);
            await packet.SendToAllAsync(LimboStates.NotLimbo);
        }

        public async ValueTask HandleJoinGame(ClientPlayer sender)
        {
            // Check if the IP of the player is banned.
            if (_bannedIps.Contains(sender.Client.Connection.EndPoint.Address))
            {
                await sender.SendDisconnectReason(DisconnectReason.Banned);
                return;
            }
            
            // Check if;
            // - The player is already in this game.
            // - The game is full.
            if (sender.Game != this && _players.Count >= Options.MaxPlayers)
            {
                await sender.SendDisconnectReason(DisconnectReason.GameFull);
                return;
            }
            
            // Check current player state.
            if (sender.Limbo == LimboStates.NotLimbo)
            {
                await sender.SendDisconnectReason(DisconnectReason.Custom, "Invalid limbo state while joining.");
                return;
            }
            
            switch (GameState)
            {
                case GameStates.NotStarted:
                    await HandleJoinGameNew(sender);
                    break;
                case GameStates.Ended:
                    await HandleJoinGameNext(sender);
                    break;
                case GameStates.Started:
                    await sender.SendDisconnectReason(DisconnectReason.GameStarted);
                    return;
                case GameStates.Destroyed:
                    await sender.SendDisconnectReason(DisconnectReason.Custom, DisconnectMessages.Destroyed);
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async ValueTask HandleEndGame(IMessageReader message)
        {
            GameState = GameStates.Ended;
            
            // Broadcast end of the game.
            using (var packet = CreateMessage(MessageType.Reliable))
            {
                message.CopyTo(packet);
                await packet.SendToAllAsync(LimboStates.NotLimbo);
            }
            
            // Put all players in the correct limbo state.
            foreach (var player in _players)
            {
                player.Value.Limbo = LimboStates.PreSpawn;
            }
        }

        public async ValueTask HandleAlterGame(IMessageReader message, ClientPlayer sender, bool isPublic)
        {
            IsPublic = isPublic;

            using var packet = CreateMessage(MessageType.Reliable);
            message.CopyTo(packet);
            await packet.SendToAllExceptAsync(LimboStates.NotLimbo, sender.Client.Id);
        }
        
        public async ValueTask HandleRemovePlayer(int playerId, DisconnectReason reason)
        {
            await PlayerRemove(playerId);

            // It's possible that the last player was removed, so check if the game is still around.
            if (GameState == GameStates.Destroyed)
            {
                return;
            }

            using var packet = CreateMessage(MessageType.Reliable);
            WriteRemovePlayerMessage(packet, false, playerId, reason);
            await packet.SendToAllExceptAsync(LimboStates.NotLimbo, playerId);
        }

        public async ValueTask HandleKickPlayer(int playerId, bool isBan)
        {
            Logger.Information("{0} - Player {1} has left.", Code, playerId);
            
            using (var message = CreateMessage(MessageType.Reliable))
            {
                // Send message to everyone that this player was kicked.
                WriteKickPlayerMessage(message, false, playerId, isBan);
                await message.SendToAllAsync(LimboStates.NotLimbo);

                await PlayerRemove(playerId, isBan);
                
                // Rmeove the player from everyone's game.
                WriteRemovePlayerMessage(message, true, playerId, isBan 
                    ? DisconnectReason.Banned 
                    : DisconnectReason.Kicked);
                await message.SendToAllExceptAsync(LimboStates.NotLimbo, playerId);
            }
        }
        
        private async ValueTask HandleJoinGameNew(ClientPlayer sender)
        {
            Logger.Information("{0} - Player {1} ({2}) is joining.", Code, sender.Client.Name, sender.Client.Id);
            
            // Add player to the game.
            if (sender.Game == null)
            {
                PlayerAdd(sender);
            }

            using (var message = CreateMessage(MessageType.Reliable))
            {
                WriteJoinedGameMessage(message, false, sender);
                WriteAlterGameMessage(message, false);
                
                sender.Limbo = LimboStates.NotLimbo;
                await message.SendToAsync(sender.Client);

                await BroadcastJoinMessage(message, true, sender);
            }
        }

        private async ValueTask HandleJoinGameNext(ClientPlayer sender)
        {
            Logger.Information("{0} - Player {1} ({2}) is rejoining.", Code, sender.Client.Name, sender.Client.Id);
            
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
                await HandleJoinGameNew(sender);
                
                // Pull players out of limbo.
                await CheckLimboPlayers();
                return;
            }

            sender.Limbo = LimboStates.WaitingForHost;

            using var packet = CreateMessage(MessageType.Reliable);
            
            WriteWaitForHostMessage(packet, false, sender);
            await packet.SendToAsync(sender.Client);

            await BroadcastJoinMessage(packet, true, sender);
        }
    }
}