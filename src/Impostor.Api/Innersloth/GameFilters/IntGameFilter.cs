using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Impostor.Api.Innersloth.GameFilters
{
    [Serializable]
    public class IntGameFilter : ISubFilter
    {
        [JsonPropertyName("FilterType")]
        public string FilterType { get; } = "int";

        [JsonPropertyName("AcceptedValues")]
        public required List<int> AcceptedValues { get; set; }

        [JsonPropertyName("OptionEnum")]
        public required Int32OptionNames OptionEnum { get; set; }
    }
}
