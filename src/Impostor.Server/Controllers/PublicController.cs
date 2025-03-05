using System.Linq;
using System.Text.Json;
using Impostor.Api.Games;
using Impostor.Api.Games.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Manager;
using Impostor.Server.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Impostor.Server.Controllers;

[ApiController]
[Route("api/public/[action]")]
public sealed class PublicController(
    IGameManager gameManager,
    IClientManager clientManager,
    INetListenerManager listenerManager
    ) : ControllerBase
{
    [HttpGet]
    public IActionResult RoomCount()
    {
        return Ok(gameManager.GetGameCount(MapFlags.All));
    }

    [HttpGet]
    public IActionResult PlayerCount()
    {
        return Ok(clientManager.Clients.Count());
    }

    [HttpGet]
    public IActionResult GameList()
    {
        var list = gameManager.Games.Select(game => new
        {
            game.Code,
            game.PlayerCount,
            MaxPlayer = (int)game.Options.MaxPlayers,
            State = game.GameState,
            Map = (MapFlags)game.Options.Map,
            Host = game.Host?.Client.Name ?? "Unknown",
        });

        return list.OkJson();
    }

    [HttpGet]
    public IActionResult ImpostorInfo()
    {
        return new
        {
            DotnetUtils.Version,
            DotnetUtils.Environment,
            DotnetUtils.IsDev,
        }.OkJson();
    }

    [HttpGet]
    public IActionResult AvailableListener()
    {
        var listener = listenerManager.GetAvailableListener();
        if (listener == null)
            return BadRequest("No available listener");
        
        return Ok(new
        {
            listener.PublicIp,
            listener.PublicPort,
            listener.IsHttps,
        }.OkJson());
    }
}
