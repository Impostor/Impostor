using System.Text.Json.Serialization;

namespace SelfHttpMatchmaker.Types;

public class GamesListMetadata
{
    [JsonPropertyName("allGamesCount")]
    public int AllGamesCount　{ get; set; }
    
    [JsonPropertyName("matchingGamesCount")]
    public int MatchingGamesCount { get; set; }
}
