using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Impostor.Api.Innersloth;

namespace SelfHttpMatchmaker.Types;

[method: SetsRequiredMembers]
public class MatchmakerError(DisconnectReason reason)
{
    [JsonPropertyName("Reason")] public required DisconnectReason Reason { get; init; } = reason;
}
