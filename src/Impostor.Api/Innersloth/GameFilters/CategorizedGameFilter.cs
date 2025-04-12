using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Impostor.Api.Innersloth.GameFilters
{
    [Serializable]
    public class CategorizedGameFilter : ISubFilter
    {
        [JsonPropertyName("FilterType")]
        public string FilterType { get; } = "cat";

        [JsonPropertyName("AcceptedValues")]
        public required List<int> AcceptedValues { get; set; }

        [JsonPropertyName("OptionEnum")]
        public required CategorizedOptionNames OptionEnum { get; set; }
    }
}
