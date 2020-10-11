using System.Linq;
using System.Threading.Tasks;
using Impostor.Server.Exceptions;
using Impostor.Server.Net.Messages;
using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Net.State
{
    internal partial class Game
    {
        private void PlayerAdd(IClientPlayer player)
        {
            // Store player.
            if (!_players.TryAdd(player.Client.Id, player))
            {
                throw new AmongUsException("Failed to add player to game.");
            }

            // Assign hostId if none is set.
            if (HostId == -1)
            {
                HostId = player.Client.Id;
            }
        }

        private async ValueTask<bool> PlayerRemove(int playerId, bool isBan = false)
        {
            if (!_players.TryRemove(playerId, out var player))
            {
                return false;
            }

            Logger.Information("{0} - Player {1} ({2}) has left.", Code, player.Client.Name, playerId);

            player.Client.Player = null;

            // Game is empty, remove it.
            if (_players.IsEmpty)
            {
                GameState = GameStates.Destroyed;

                // Remove instance reference.
                await _gameManager.RemoveAsync(Code);
                return true;
            }

            // Host migration.
            if (HostId == playerId)
            {
                await MigrateHost();
            }

            if (isBan && player.Client.Connection != null)
            {
                _bannedIps.Add(player.Client.Connection.EndPoint.Address);
            }

            return true;
        }

        private async ValueTask MigrateHost()
        {
            // Pick the first player as new host.
            var host = _players
                .Select(p => p.Value)
                .FirstOrDefault(p => !p.Client.IsBot);

            if (host == null)
            {
                await EndAsync();
                return;
            }

            HostId = host.Client.Id;
            Logger.Information("{0} - Assigned {1} ({2}) as new host.", Code, host.Client.Name, host.Client.Id);

            // Check our current game state.
            if (GameState == GameStates.Ended && host.Limbo == LimboStates.WaitingForHost)
            {
                GameState = GameStates.NotStarted;

                // Spawn the host.
                await HandleJoinGameNew(host, false);

                // Pull players out of limbo.
                await CheckLimboPlayers();
            }
        }

        private async ValueTask CheckLimboPlayers()
        {
            using var message = CreateMessage(MessageType.Reliable);

            foreach (var (_, player) in _players.Where(x => x.Value.Limbo == LimboStates.WaitingForHost))
            {
                WriteJoinedGameMessage(message, true, player);
                WriteAlterGameMessage(message, false, IsPublic);

                player.Limbo = LimboStates.NotLimbo;
                await message.SendToAsync(player.Client);
            }
        }
    }
}