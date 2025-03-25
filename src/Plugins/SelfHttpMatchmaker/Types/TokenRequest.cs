using System.Text.Json.Serialization;
using Impostor.Api.Innersloth;

namespace SelfHttpMatchmaker.Types;

/// <summary>
///     Body of the token request endpoint.
/// </summary>
public class TokenRequest
{
    [JsonPropertyName("Puid")] public required string ProductUserId { get; init; }

    [JsonPropertyName("Username")] public required string Username { get; init; }

    [JsonPropertyName("ClientVersion")] public required int ClientVersion { get; init; }

    [JsonPropertyName("Language")] public required Language Language { get; init; }
}
