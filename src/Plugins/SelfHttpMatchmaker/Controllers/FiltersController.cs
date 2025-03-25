using System.Net;
using Impostor.Api.Config;
using Impostor.Api.Games.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Manager;
using Impostor.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using SelfHttpMatchmaker.Types;

namespace SelfHttpMatchmaker.Controllers;

[ApiController]
public class FiltersController(INetListenerManager listenerManager, IGameManager gameManager) : ControllerBase
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
    
    [HttpGet("api/filters")]
    public IActionResult GetFilters()
    {
        return Ok(new PermittedFilters()
        {
            // TODO: Add filters
            Filters = [],
        });
    }

    [HttpGet("api/filtertags")]
    public IActionResult GetFilterTags()
    {
        // TODO: Add filter tags
        return Ok(new HashSet<string>());
    }

    [HttpGet("api/games/filtered")]
    public IActionResult GetFilteredGames()
    {
        // TODO: Add filter

        var list = gameManager.Games.Where(game => game.IsPublic)
            .Select(game => GameListing.From(game, HostServer.Ip, HostServer.Port)).ToList();
        var res = new FindGamesListFilteredResponse
        {
            Games = list,
            Metadata = new GamesListMetadata
            {
                AllGamesCount = list.Count,
                MatchingGamesCount = list.Count,
            },
        };
        return Ok(res);
    }
}
