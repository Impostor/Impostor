using Microsoft.AspNetCore.Mvc;

namespace Impostor.Server.Http;

/// <summary>
/// Generate a diagnostic page to show that the Impostor HTTP server is working.
/// </summary>
[Route("/")]
public sealed class HelloController : ControllerBase
{
    [HttpGet]
    public IActionResult GetHello()
    {
        return Ok(@"Impostor is running, please configure your Among Us to connect to a game
To generate a region file, go to https://impostor.github.io/Impostor");
    }
}
