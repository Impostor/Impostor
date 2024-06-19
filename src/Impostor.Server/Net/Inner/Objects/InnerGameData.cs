using System.Collections.Concurrent;
using System.Collections.Generic;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerGameData : IInnerGameData
    {
        private readonly ConcurrentDictionary<byte, InnerPlayerInfo> _allPlayers = new();

        public int PlayerCount => _allPlayers.Count;

        public IReadOnlyDictionary<byte, InnerPlayerInfo> Players => _allPlayers;

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
            return _allPlayers.TryAdd(playerInfo.PlayerId, playerInfo);
        }

        internal void RemovePlayer(byte playerId)
        {
            _allPlayers.TryRemove(playerId, out _);
        }
    }
}
