using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Impostor.Api.Config;
using Impostor.Api.Games;
using Impostor.Api.Games.Managers;
using Impostor.Api.Innersloth;
using Impostor.Server.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Http;

/// <summary>
/// This controller has method to get a list of public games, join by game and create new games.
/// </summary>
[Route("/api/games")]
[ApiController]
public sealed class GamesController : ControllerBase
{
    private readonly IGameManager _gameManager;
    private readonly ListingManager _listingManager;
    private readonly HostServer _hostServer;

    /// <summary>
    /// Initializes a new instance of the <see cref="GamesController"/> class.
    /// </summary>
    /// <param name="gameManager">GameManager containing a list of games.</param>
    /// <param name="listingManager">ListingManager responsible for filtering.</param>
    /// <param name="serverConfig">Impostor configuration section containing the public ip address of this server.</param>
    public GamesController(IGameManager gameManager, ListingManager listingManager, IOptions<ServerConfig> serverConfig)
    {
        _gameManager = gameManager;
        _listingManager = listingManager;
        var config = serverConfig.Value;
        _hostServer = HostServer.From(IPAddress.Parse(config.ResolvePublicIp()), config.PublicPort);
    }

    /// <summary>
    /// Get a list of active games.
    /// </summary>
    /// <param name="mapId">Maps that are requested.</param>
    /// <param name="lang">Preferred chat language.</param>
    /// <param name="numImpostors">Amount of impostors. 0 is any.</param>
    /// <param name="authorization">Authorization header containing the matchmaking token.</param>
    /// <returns>An array of game listings.</returns>
    [HttpGet]
    public IActionResult Index(int mapId, GameKeywords lang, int numImpostors, [FromHeader] AuthenticationHeaderValue authorization)
    {
        // NOTE: this method is no longer used by Among Us 16.0.0 and is only kept for backwards compatibility
        if (authorization.Scheme != "Bearer" || authorization.Parameter == null)
        {
            return BadRequest();
        }

        var token = JsonSerializer.Deserialize<TokenController.Token>(Convert.FromBase64String(authorization.Parameter));
        if (token == null)
        {
            return BadRequest();
        }

        var clientVersion = new GameVersion(token.Content.ClientVersion);

        var listings = _listingManager.FindListings(HttpContext, mapId, numImpostors, lang, clientVersion);
        return Ok(listings.Select(GameListing.From));
    }

    /// <summary>
    /// Get the address a certain game is hosted at.
    /// </summary>
    /// <param name="gameId">The id of the game that should be retrieved.</param>
    /// <returns>The server this game is hosted on.</returns>
    [HttpPost]
    public IActionResult Post(int gameId)
    {
        // NOTE: this method is no longer used by Among Us 16.0.0 and is only kept for backwards compatibility
        var code = new GameCode(gameId);
        var game = _gameManager.Find(code);

        // If the game was not found, print an error message.
        if (game == null)
        {
            return NotFound(new MatchmakerResponse(new MatchmakerError(DisconnectReason.GameNotFound)));
        }

        return Ok(HostServer.From(game.PublicIp));
    }

    /// <summary>
    /// Get the address to host a new game on.
    /// </summary>
    /// <returns>The address of this server.</returns>
    [HttpPut]
    public IActionResult Put()
    {
        return Ok(_hostServer);
    }

    [HttpGet("{gameId}")]
    public IActionResult Show([FromRoute] int gameId)
    {
        var code = new GameCode(gameId);
        var game = _gameManager.Find(code);

        // If the game was not found, print an error message.
        if (game == null)
        {
            return NotFound(new FindGameByCodeResponse(new MatchmakerError(DisconnectReason.GameNotFound)));
        }

        return Ok(new FindGameByCodeResponse(GameListing.From(game)));
    }

    [HttpGet("filtered")]
    public IActionResult ShowFilteredLobbies()
    {
        try
        {
            var publicGames = _gameManager.Games
                .Where(game => game.IsPublic &&
                              game.GameState == GameStates.NotStarted &&
                              game.PlayerCount < game.Options.MaxPlayers)
                .ToList();

            var gameListings = publicGames.Select(GameListing.From).ToList();

            var response = new
            {
                Games = gameListings,
                Metadata = new
                {
                    allGamesCount = _gameManager.Games.Count(),
                    matchingGamesCount = gameListings.Count,
                },
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve game list", details = ex.Message });
        }
    }

    private static uint ConvertAddressToNumber(IPAddress address)
    {
#pragma warning disable CS0618 // Among Us only supports IPv4
        return (uint)address.Address;
#pragma warning restore CS0618
    }

    private class HostServer
    {
        [JsonPropertyName("Ip")]
        public required long Ip { get; init; }

        [JsonPropertyName("Port")]
        public required ushort Port { get; init; }

        public static HostServer From(IPAddress ipAddress, ushort port)
        {
            return new HostServer
            {
                Ip = ConvertAddressToNumber(ipAddress),
                Port = port,
            };
        }

        public static HostServer From(IPEndPoint endPoint)
        {
            return From(endPoint.Address, (ushort)endPoint.Port);
        }
    }

    private class MatchmakerResponse
    {
        [SetsRequiredMembers]
        public MatchmakerResponse(MatchmakerError error)
        {
            Errors = new[] { error };
        }

        [JsonPropertyName("Errors")]
        public required MatchmakerError[] Errors { get; init; }
    }

    private class MatchmakerError
    {
        [SetsRequiredMembers]
        public MatchmakerError(DisconnectReason reason)
        {
            Reason = reason;
        }

        [JsonPropertyName("Reason")]
        public required DisconnectReason Reason { get; init; }
    }

    private class FindGameByCodeResponse
    {
        [SetsRequiredMembers]
        public FindGameByCodeResponse(MatchmakerError error) => (Errors, Game) = (new[] { error }, null);

        [SetsRequiredMembers]
        public FindGameByCodeResponse(GameListing game) => (Errors, Game) = (null, game);

        [JsonPropertyName("Errors")]
        public required MatchmakerError[]? Errors { get; init; }

        [JsonPropertyName("Game")]
        public required GameListing? Game { get; init; }
    }

    private class GameListing
    {
        [JsonPropertyName("IP")]
        public required uint Ip { get; init; }

        [JsonPropertyName("Port")]
        public required ushort Port { get; init; }

        [JsonPropertyName("GameId")]
        public required int GameId { get; init; }

        [JsonPropertyName("PlayerCount")]
        public required int PlayerCount { get; init; }

        [JsonPropertyName("HostName")]
        public required string HostName { get; init; }

        [JsonPropertyName("TrueHostName")]
        public required string TrueHostName { get; init; }

        [JsonPropertyName("HostPlatformName")]
        public required string HostPlatformName { get; init; }

        [JsonPropertyName("Platform")]
        public required Platforms Platform { get; init; }

        [JsonPropertyName("QuickChat")]
        public required QuickChatModes QuickChat { get; init; }

        [JsonPropertyName("Age")]
        public required int Age { get; init; }

        [JsonPropertyName("MaxPlayers")]
        public required int MaxPlayers { get; init; }

        [JsonPropertyName("NumImpostors")]
        public required int NumImpostors { get; init; }

        [JsonPropertyName("MapId")]
        public required MapTypes MapId { get; init; }

        [JsonPropertyName("Language")]
        public required GameKeywords Language { get; init; }

        [JsonPropertyName("Options")]
        public required string Options { get; init; }

        public static GameListing From(IGame game)
        {
            var platform = game.Host?.Client.PlatformSpecificData;

            return new GameListing
            {
                Ip = ConvertAddressToNumber(game.PublicIp.Address),
                Port = (ushort)game.PublicIp.Port,
                GameId = game.Code,
                PlayerCount = game.PlayerCount,
                HostName = game.DisplayName ?? game.Host?.Client.Name ?? "Unknown host",
                TrueHostName = game.DisplayName ?? game.Host?.Client.Name ?? "Unknown host",
                HostPlatformName = platform?.PlatformName ?? string.Empty,
                Platform = platform?.Platform ?? Platforms.Unknown,
                QuickChat = game.Host?.Client.ChatMode ?? QuickChatModes.QuickChatOnly,
                Age = 0,
                MaxPlayers = game.Options.MaxPlayers,
                NumImpostors = game.Options.NumImpostors,
                MapId = game.Options.Map,
                Language = game.Options.Keywords,
                Options = game.Options.ToBase64String(),
            };
        }
    }
}
