using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Impostor.Api.Innersloth.GameFilters
{
    [Serializable]
    public class GameFiltersList
    {
        [JsonPropertyName("FilterSets")]
        public required List<GameFilterSet> FilterSets { get; set; }
    }
}
