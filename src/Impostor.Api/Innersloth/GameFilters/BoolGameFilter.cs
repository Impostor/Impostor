using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Impostor.Api.Innersloth.GameFilters
{
    [Serializable]
    public class BoolGameFilter : ISubFilter
    {
        [JsonPropertyName("FilterType")]
        public string FilterType { get; } = "bool";

        [JsonPropertyName("AcceptedValues")]
        public required List<bool> AcceptedValues { get; set; }

        [JsonPropertyName("OptionEnum")]
        public required BoolOptionNames OptionEnum { get; set; }
    }
}
