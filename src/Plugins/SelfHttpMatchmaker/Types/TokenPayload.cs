using System.Text.Json.Serialization;

namespace SelfHttpMatchmaker.Types;

/// <summary>
///     Actual token contents.
/// </summary>
public sealed class TokenPayload
{
    private static readonly DateTime DefaultExpiryDate = new(2012, 12, 21);

    [JsonPropertyName("Puid")] public required string ProductUserId { get; init; }

    [JsonPropertyName("ClientVersion")] public required int ClientVersion { get; init; }

    [JsonPropertyName("ExpiresAt")] public DateTime ExpiresAt { get; init; } = DefaultExpiryDate;
}
