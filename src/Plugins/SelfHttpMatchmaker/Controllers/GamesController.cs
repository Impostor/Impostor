using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Impostor.Api.Config;
using Impostor.Api.Games;
using Impostor.Api.Games.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Manager;
using Impostor.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SelfHttpMatchmaker.Types;

namespace SelfHttpMatchmaker.Controllers;

/// <summary>
///     This controller has method to get a list of public games, join by game and create new games.
/// </summary>
[Route("/api/games")]
[ApiController]
public sealed class GamesController(
    IGameManager gameManager,
    ListingManager listingManager,
    INetListenerManager listenerManager,
    IOptions<ExtensionServerConfig> config) : ControllerBase
{
    private HostServer? _hostServer;

    private HostServer HostServer
    {
        get
        {
            if (_hostServer != null)
            {
                return _hostServer;
            }

            _hostServer = HostServer.From(IPAddress.Parse(Listener.PublicIp.ResolveIp()), Listener.PublicPort);
            return _hostServer;
        }
    }

    private ListenerConfig Listener
    {
        get => listenerManager.GetAvailableListener() ?? throw new InvalidOperationException();
    }

    /// <summary>
    ///     Get a list of active games.
    /// </summary>
    /// <param name="mapId">Maps that are requested.</param>
    /// <param name="lang">Preferred chat language.</param>
    /// <param name="numImpostors">Amount of impostors. 0 is any.</param>
    /// <param name="authorization">Authorization header containing the matchmaking token.</param>
    /// <returns>An array of game listings.</returns>
    [HttpGet]
    public IActionResult Index(int mapId, GameKeywords lang, int numImpostors,
        [FromHeader] AuthenticationHeaderValue authorization)
    {
        if (authorization.Scheme != "Bearer" || authorization.Parameter == null)
        {
            return BadRequest();
        }

        var token =
            JsonSerializer.Deserialize<Token>(Convert.FromBase64String(authorization.Parameter));
        if (token == null)
        {
            return BadRequest();
        }

        var clientVersion = new GameVersion(token.Content.ClientVersion);

        var listings = listingManager.FindListings(HttpContext, mapId, numImpostors, lang, clientVersion);

        return Ok(listings.Select(n => GameListing.From(n, HostServer.Ip, HostServer.Port)));
    }

    /// <summary>
    ///     Get the address a certain game is hosted at.
    /// </summary>
    /// <param name="gameId">The id of the game that should be retrieved.</param>
    /// <returns>The server this game is hosted on.</returns>
    [HttpPost]
    public IActionResult Post(int gameId)
    {
        var code = new GameCode(gameId);
        var game = gameManager.Find(code);

        // If the game was not found, print an error message.
        if (game == null)
        {
            return NotFound(new MatchmakerResponse(new MatchmakerError(DisconnectReason.GameNotFound)));
        }

        return Ok(HostServer);
    }

    /// <summary>
    ///     Get the address to host a new game on.
    /// </summary>
    /// <returns>The address of this server.</returns>
    [HttpPut]
    public IActionResult Put()
    {
        return Ok(HostServer);
    }

    [HttpGet("{gameId:int}")]
    public IActionResult FindGameInfo(int gameId)
    {
        var code = GameCode.From(gameId);
        var game = gameManager.Find(code);
        if (game == null)
        {
            return NotFound(new MatchmakerResponse(new MatchmakerError(DisconnectReason.GameNotFound)));
        }

        var res = new FindGameByCodeResponse
        {
            Errors = [],
            Game = GameListing.From(game, HostServer.Ip, HostServer.Port),
            Region = StringNames.NoTranslation,
            UntranslatedRegion = config.Value.RegionName,
        };
        return Ok(res);
    }
}
