using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SelfHttpMatchmaker.Types;

namespace SelfHttpMatchmaker.Controllers;

/// <summary>
///     This controller has a method to get an auth token.
/// </summary>
[Route("/api/user")]
[ApiController]
public sealed class TokenController : ControllerBase
{
    /// <summary>
    ///     Get an authentication token.
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
        return Ok(Convert.ToBase64String(serialized));
    }
}
