using System.Reflection;
using System.Text.Json.Serialization;
using Impostor.Api.Config;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Manager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Http;

/// <summary>
/// Generate a diagnostic page to show that the Impostor HTTP server is working.
/// </summary>
[Route("/")]
public sealed class HelloController : ControllerBase
{
    private static bool _shownHello = false;
    private readonly ILogger<HelloController> _logger;

    private readonly ICompatibilityManager _compatibilityManager;
    private readonly CompatibilityConfig _compatibilityConfig;

    public HelloController(
        ILogger<HelloController> logger,
        ICompatibilityManager compatibilityManager,
        IOptions<CompatibilityConfig> compatibilityConfig)
    {
        _logger = logger;
        _compatibilityManager = compatibilityManager;
        _compatibilityConfig = compatibilityConfig.Value;
    }

    [HttpGet]
    public IActionResult GetHello()
    {
        if (!_shownHello)
        {
            _shownHello = true;
            _logger.LogInformation("Impostor's Http server is reachable (this message is only printed once per start)");
        }

        return Ok(
            """
            Impostor is running, please configure your Among Us to connect to a game
            To generate a region file, go to https://impostor.github.io/Impostor
            """
        );
    }

    [HttpPost]
    public IActionResult PostStatus(int clientVersion)
    {
        var version = new GameVersion(clientVersion);
        var result = _compatibilityManager.CanConnectToServer(version);

        var status = result is ICompatibilityManager.VersionCompareResult.Compatible ?
            ServerStatus.VersionCheck.Ok : ServerStatus.VersionCheck.Ng;

        if (status is ServerStatus.VersionCheck.Ng &&
            result is ICompatibilityManager.VersionCompareResult.ServerTooOld &&
            _compatibilityConfig.AllowFutureGameVersions)
        {
            status = ServerStatus.VersionCheck.MayBeOk;
        }

        var response = new ServerStatus()
        {
            Status = status.ToString(),
            Version = Assembly.GetAssembly(this.GetType())?.GetName()?.Version?.ToString(),
        };
        return Ok(response);
    }

    private class ServerStatus
    {
        public enum VersionCheck
        {
            Ng,
            MayBeOk,
            Ok,
        }

        [JsonPropertyName("status")]
        public required string Status { get; init; }

        [JsonPropertyName("version")]
        public required string? Version { get; init; }
    }
}
