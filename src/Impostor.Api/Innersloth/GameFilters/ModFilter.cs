using System;
using System.Text.Json.Serialization;

// https://github.com/Innersloth-LLC/AmongUsModdingInformation
namespace Impostor.Api.Innersloth.GameFilters
{
    [Serializable]
    public class ModFilter : ISubFilter
    {
        [JsonPropertyName("FilterType")]
        public string FilterType { get; } = "mod";

        [JsonPropertyName("AcceptedValues")]
        public required Guid AcceptedValues { get; set; }
    }
}
