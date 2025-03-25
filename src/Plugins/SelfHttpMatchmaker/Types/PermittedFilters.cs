using System.Text.Json.Serialization;
using Impostor.Api.Innersloth;

namespace SelfHttpMatchmaker.Types;

public class PermittedFilters
{
    [JsonPropertyName("filters")]
    public List<Filters> Filters { get; set; }
}
