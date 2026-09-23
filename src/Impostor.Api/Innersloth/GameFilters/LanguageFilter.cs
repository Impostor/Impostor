using System;
using System.Text.Json.Serialization;

namespace Impostor.Api.Innersloth.GameFilters
{
    [Serializable]
    public class LanguageFilter : ISubFilter
    {
        [JsonPropertyName("FilterType")]
        public string FilterType { get; } = "languages";

        [JsonPropertyName("AcceptedValues")]
        public required uint AcceptedValues { get; set; }
    }
}
