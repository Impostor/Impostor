using System.Linq;
using Hazel;
using Impostor.Server.Exceptions;
using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Net.State
{
    internal partial class Game
    {
        private void PlayerAdd(ClientPlayer player)
        {
            // Store player.
            if (!_players.TryAdd(player.Client.Id, player))
            {
                throw new AmongUsException("Failed to add player to game.");
            }
            
            // Assign player to this game for future packets.
            player.Game = this;

            // Assign hostId if none is set.
            if (HostId == -1)
            {
                HostId = player.Client.Id;
            }
        }

        private bool PlayerRemove(int playerId, out ClientPlayer player)
        {
            if (!_players.TryRemove(playerId, out player))
            {
                return false;
            }

            player.Limbo = LimboStates.PreSpawn;
            player.Game = null;
            
            Logger.Information("{0} - Player {1} ({2}) has left.", CodeStr, player.Client.Name, playerId);
            
            // Game is empty, remove it.
            if (_players.Count == 0)
            {
                GameState = GameStates.Destroyed;

                // Remove instance reference.
                _gameManager.Remove(Code);
                return true;
            }

            // Host migration.
            if (HostId == playerId)
            {
                MigrateHost();
            }

            return true;
        }

        private void MigrateHost()
        {
            // Pick the first player as new host.
            var host = _players.First().Value;
            
            HostId = host.Client.Id;
            Logger.Information("{0} - Assigned {1} ({2}) as new host.", CodeStr, host.Client.Name, host.Client.Id);
            
            // Check our current game state.
            if (GameState == GameStates.Ended && host.Limbo == LimboStates.WaitingForHost)
            {
                GameState = GameStates.NotStarted;
                
                // Spawn the host.
                HandleJoinGameNew(host);
                
                // Pull players out of limbo.
                CheckLimboPlayers();
            }
        }

        private void CheckLimboPlayers()
        {
            using (var message = MessageWriter.Get(SendOption.Reliable))
            {
                foreach (var (_, player) in _players.Where(x => x.Value.Limbo == LimboStates.WaitingForHost))
                {
                    WriteJoinedGameMessage(message, true, player);
                    WriteAlterGameMessage(message, false);
                        
                    player.Limbo = LimboStates.NotLimbo;
                    player.Client.Send(message);
                }
            }
        }
    }
}