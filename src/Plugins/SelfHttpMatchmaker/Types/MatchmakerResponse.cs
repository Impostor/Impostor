using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace SelfHttpMatchmaker.Types;

[method: SetsRequiredMembers]
public class MatchmakerResponse(MatchmakerError error)
{
    [JsonPropertyName("Errors")] public required MatchmakerError[] Errors { get; init; } = [error];
}
