using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Impostor.Api.Innersloth;
using Impostor.Server.Account;
using Microsoft.AspNetCore.Mvc;

namespace Impostor.Server.Http;

/// <summary>
/// This controller has a method to get an auth token.
/// </summary>
[Route("/api/user")]
[ApiController]
public sealed class TokenController : ControllerBase
{
    // Backdated so the token is already expired for any other server that receives it.
    private static readonly TimeSpan ExpiryBackdate = TimeSpan.FromHours(2);

    private readonly GameCodeAuthManager _accounts;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenController"/> class.
    /// </summary>
    /// <param name="accounts">Session store used to bind tokens to game connections.</param>
    public TokenController(GameCodeAuthManager accounts)
    {
        _accounts = accounts;
    }

    /// <summary>
    /// Get an authentication token.
    /// </summary>
    /// <param name="request">Token parameters that need to be put into the token.</param>
    /// <param name="authorization">Authorization header of the request, if any.</param>
    /// <returns>A bare minimum authentication token that the client will accept.</returns>
    [HttpPost]
    public IActionResult GetToken([FromBody] TokenRequest request, [FromHeader(Name = "Authorization")] string? authorization = null)
    {
        var hash = GameCodeAuthManager.HashPrefix + "was_here";

        if (_accounts.Enabled)
        {
            var bearer = authorization != null && authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? authorization["Bearer ".Length..]
                : null;

            hash = _accounts.CreateSession(request.ProductUserId, request.Username, bearer);
        }

        var token = new Token
        {
            Content = new TokenPayload
            {
                ProductUserId = request.ProductUserId,
                ClientVersion = request.ClientVersion,
                ExpiresAt = DateTime.UtcNow - ExpiryBackdate,
            },
            Hash = hash,
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
