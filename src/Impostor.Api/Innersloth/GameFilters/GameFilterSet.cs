using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Impostor.Api.Innersloth.GameFilters
{
    [Serializable]
    public class GameFilterSet
    {
        [JsonConstructor]
        public GameFilterSet(GameModes gameMode, List<GameFilter> filters)
        {
            GameMode = gameMode;
            Filters = filters;
        }

        [JsonPropertyName("GameMode")]
        public required GameModes GameMode { get; set; }

        [JsonPropertyName("Filters")]
        public required List<GameFilter> Filters { get; set; }
    }
}
