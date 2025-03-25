using System.Text.Json.Serialization;

namespace SelfHttpMatchmaker.Types;

public class FindGamesListFilteredResponse
{
    [JsonPropertyName("games")]
    public List<GameListing> Games { get; set; }
    
    [JsonPropertyName("metadata")]
    public GamesListMetadata Metadata { get; set; }
}
