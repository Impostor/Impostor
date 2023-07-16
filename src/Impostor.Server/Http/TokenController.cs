using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Impostor.Api.Innersloth;
using Microsoft.AspNetCore.Mvc;

namespace Impostor.Server.Http;

/// <summary>
/// This controller has a method to get an auth token.
/// </summary>
[Route("/api/user")]
[ApiController]
public sealed class TokenController : ControllerBase
{
    /// <summary>
    /// Get an authentication token.
    /// </summary>
    /// <param name="request">Token parameters that need to be put into the token.</param>
    /// <returns>A bare minimum authentication token that the client will accept.</returns>
    [HttpPost]
    public IActionResult GetToken([FromBody] TokenRequest request)
    {
        var token = new Token
        {
            Content = new TokenPayload
            {
                ProductUserId = request.ProductUserId,
                ClientVersion = request.ClientVersion,
            },
            Hash = "impostor_was_here",
        };

        // Wrap into a Base64 sandwich
        var serialized = JsonSerializer.SerializeToUtf8Bytes(token);
        return this.Ok(Convert.ToBase64String(serialized));
    }

    /// <summary>
    /// Body of the token request endpoint.
    /// </summary>
    public class TokenRequest
    {
        [JsonPropertyName("Puid")]
        public required string ProductUserId { get; init; }

        [JsonPropertyName("Username")]
        public required string Username { get; init; }

        [JsonPropertyName("ClientVersion")]
        public required int ClientVersion { get; init; }

        [JsonPropertyName("Language")]
        public required Language Language { get; init; }
    }

    /// <summary>
    /// Token that is returned to the user with a "signature".
    /// </summary>
    public sealed class Token
    {
        [JsonPropertyName("Content")]
        public required TokenPayload Content { get; init; }

        [JsonPropertyName("Hash")]
        public required string Hash { get; init; }
    }

    /// <summary>
    /// Actual token contents.
    /// </summary>
    public sealed class TokenPayload
    {
        private static readonly DateTime DefaultExpiryDate = new(2012, 12, 21);

        [JsonPropertyName("Puid")]
        public required string ProductUserId { get; init; }

        [JsonPropertyName("ClientVersion")]
        public required int ClientVersion { get; init; }

        [JsonPropertyName("ExpiresAt")]
        public DateTime ExpiresAt { get; init; } = DefaultExpiryDate;
    }
}
