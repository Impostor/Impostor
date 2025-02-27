using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Impostor.Api.Config;
using Impostor.Api.Games;
using Impostor.Api.Games.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SelfHttpMatchmaker.Controllers;

/// <summary>
///     This controller has method to get a list of public games, join by game and create new games.
/// </summary>
[Route("/api/games")]
[ApiController]
public sealed class GamesController(
    IGameManager gameManager,
    ListingManager listingManager,
    IOptions<ServerConfig> serverConfig) : ControllerBase
{
    private readonly ServerConfig _config = serverConfig.Value;
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
        get => _config.Listeners.FirstOrDefault() ?? throw new InvalidOperationException();
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
            JsonSerializer.Deserialize<TokenController.Token>(Convert.FromBase64String(authorization.Parameter));
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
}

public class HostServer
{
    [JsonPropertyName("Ip")] public required long Ip { get; init; }

    [JsonPropertyName("Port")] public required ushort Port { get; init; }

    public static HostServer From(IPAddress ipAddress, ushort port)
    {
        return new HostServer
        {
#pragma warning disable CS0618 // 类型或成员已过时
            Ip = ipAddress.Address,
#pragma warning restore CS0618 // 类型或成员已过时
            Port = port,
        };
    }
}

[method: SetsRequiredMembers]
public class MatchmakerResponse(MatchmakerError error)
{
    [JsonPropertyName("Errors")] public required MatchmakerError[] Errors { get; init; } = new[] { error };
}

[method: SetsRequiredMembers]
public class MatchmakerError(DisconnectReason reason)
{
    [JsonPropertyName("Reason")] public required DisconnectReason Reason { get; init; } = reason;
}

public class GameListing
{
    [JsonPropertyName("IP")] public required long Ip { get; init; }

    [JsonPropertyName("Port")] public required ushort Port { get; init; }

    [JsonPropertyName("GameId")] public required int GameId { get; init; }

    [JsonPropertyName("PlayerCount")] public required int PlayerCount { get; init; }

    [JsonPropertyName("HostName")] public required string HostName { get; init; }

    [JsonPropertyName("HostPlatformName")] public required string HostPlatformName { get; init; }

    [JsonPropertyName("Platform")] public required Platforms Platform { get; init; }

    [JsonPropertyName("Age")] public required int Age { get; init; }

    [JsonPropertyName("MaxPlayers")] public required int MaxPlayers { get; init; }

    [JsonPropertyName("NumImpostors")] public required int NumImpostors { get; init; }

    [JsonPropertyName("MapId")] public required MapTypes MapId { get; init; }

    [JsonPropertyName("Language")] public required GameKeywords Language { get; init; }

    public static GameListing From(IGame game, long ip, ushort port)
    {
        var platform = game.Host?.Client.PlatformSpecificData;

        return new GameListing
        {
            Ip = ip,
            Port = port,
            GameId = game.Code,
            PlayerCount = game.PlayerCount,
            HostName = game.DisplayName ?? game.Host?.Client.Name ?? "Unknown host",
            HostPlatformName = platform?.PlatformName ?? string.Empty,
            Platform = platform?.Platform ?? Platforms.Unknown,
            Age = 0,
            MaxPlayers = game.Options.MaxPlayers,
            NumImpostors = game.Options.NumImpostors,
            MapId = game.Options.Map,
            Language = game.Options.Keywords,
        };
    }
}
