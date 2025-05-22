using System;
using System.Text.Json.Serialization;

namespace Impostor.Api.Innersloth.GameFilters
{
    [Serializable]
    public class ChatModeGameFilter : ISubFilter
    {
        [JsonPropertyName("FilterType")]
        public string FilterType { get; } = "chat";

        [JsonPropertyName("AcceptedValues")]
        public required byte AcceptedValues { get; set; }
    }
}
