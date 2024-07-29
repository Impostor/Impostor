using System.Collections.Concurrent;
using System.Collections.Generic;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerGameData : IInnerGameData
    {
        private readonly ConcurrentDictionary<byte, InnerPlayerInfo> _allPlayers = new();
        private readonly ConcurrentDictionary<int, InnerPlayerInfo> _allPlayersByClientId = new();

        public int PlayerCount => _allPlayers.Count;

        public IReadOnlyDictionary<byte, InnerPlayerInfo> Players => _allPlayers;

        internal IReadOnlyDictionary<int, InnerPlayerInfo> PlayersByClientId => _allPlayersByClientId;

        public InnerPlayerInfo? GetPlayerById(byte id)
        {
            if (id == byte.MaxValue)
            {
                return null;
            }

            return _allPlayers.TryGetValue(id, out var player) ? player : null;
        }

        internal bool AddPlayer(InnerPlayerInfo playerInfo)
        {
            return _allPlayers.TryAdd(playerInfo.PlayerId, playerInfo) &&
                _allPlayersByClientId.TryAdd(playerInfo.ClientId, playerInfo);
        }

        internal void RemovePlayer(byte playerId)
        {
            _allPlayers.TryRemove(playerId, out var player);

            if (player != null)
            {
                _allPlayersByClientId.TryRemove(player.ClientId, out _);
            }
        }

        internal byte GetNextAvailablePlayerId()
        {
            for (byte i = 0; i < byte.MaxValue; i++)
            {
                if (!Players.ContainsKey(i))
                {
                    return i;
                }
            }

            return byte.MaxValue;
        }
    }
}
