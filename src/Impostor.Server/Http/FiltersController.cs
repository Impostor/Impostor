using System;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Impostor.Server.Http;

[Route("/api/filters")]
public sealed class FiltersController : ControllerBase
{
    private static readonly string[] Value = new[]
        {
                "Tags",

                // "PlayerSpeed",
                // "Roles",
                // "KillCooldown",
                // "VotingTime",
                "NumImposters",

                // "VisualTasks",
                // "AnonymousVotes",
                // "ConfirmEjects",
                // "DiscussionTime",
                // "EmergencyCooldown",
                // "NumEmergencyMeetings",
                // "NumCommonTasks",
                // "NumShortTasks",
                // "NumLongTasks",
                // "KillDistance",
        };

    [HttpGet]
    public IActionResult GetFilters([FromHeader] AuthenticationHeaderValue authorization)
    {
        if (authorization.Scheme != "Bearer" || authorization.Parameter == null)
        {
            return BadRequest();
        }

        TokenController.Token? token = null;
        try
        {
            token = JsonSerializer.Deserialize<TokenController.Token>(Convert.FromBase64String(authorization.Parameter));
        }
        catch
        {
            return BadRequest();
        }

        return Ok(new
        {
            filters = Value,
        });
    }
}
