using System.Collections.Generic;
using System.Linq;
using Impostor.Api.Config;
using Impostor.Api.Games;
using Impostor.Api.Games.Managers;
using Impostor.Api.Http;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Manager;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Http;

/// <summary>
/// Perform game listing filtering.
/// </summary>
public sealed class ListingManager
{
    private readonly IGameManager _gameManager;
    private readonly IEnumerable<IListingFilter> _listingFilters;
    private readonly ICompatibilityManager _compatibilityManager;
    private readonly CompatibilityConfig _compatibilityConfig;

    public ListingManager(IGameManager gameManager, IEnumerable<IListingFilter> listingFilters, ICompatibilityManager compatibilityManager, IOptions<CompatibilityConfig> compatibilityConfig)
    {
        _gameManager = gameManager;
        _listingFilters = listingFilters;
        _compatibilityManager = compatibilityManager;
        _compatibilityConfig = compatibilityConfig.Value;
    }

    /// <summary>
    /// Find listings that match the requested settings.
    /// </summary>
    /// <param name="ctx">The context of this http request.</param>
    /// <param name="map">The selected maps.</param>
    /// <param name="impostorCount">The amount of impostors. 0 is any.</param>
    /// <param name="language">Chat language of the game.</param>
    /// <param name="gameVersion">Game version of the client.</param>
    /// <param name="maxListings">Maximum amount of games to return.</param>
    /// <returns>Listings that match the required criteria.</returns>
    public IEnumerable<IGame> FindListings(HttpContext ctx, int map, int impostorCount, GameKeywords language, GameVersion gameVersion, int maxListings = 10)
    {
        var resultCount = 0;

        var filters = _listingFilters.Select(f => f.GetFilter(ctx)).ToArray();

        var compatibleGames = new List<IGame>();

        // We want to add 2 types of games
        // 1. Desireable games that the player wants to play (right language, right map, desired impostor amount)
        // 2. Failing that, display compatible games the player could join (public games with spots available)

        // .Where filters out games that can't be joined.
        foreach (var game in this._gameManager.Games)
        {
            if (!game.IsPublic || game.GameState != GameStates.NotStarted || game.PlayerCount >= game.Options.MaxPlayers)
            {
                continue;
            }

            if (!_compatibilityConfig.AllowVersionMixing &&
                game.Host != null &&
                _compatibilityManager.CanJoinGame(game.Host.Client.GameVersion, gameVersion) != GameJoinError.None)
            {
                continue;
            }

            if (!filters.All(filter => filter(game)))
            {
                continue;
            }

            if (IsGameDesired(game, map, impostorCount, language))
            {
                // Add to result immediately.
                yield return game;

                // Break out if we have enough.
                if (++resultCount == maxListings)
                {
                    yield break;
                }
            }
            else
            {
                // Add to result to add afterwards. Adding is pointless if we already have enough compatible games to fill the list
                if (compatibleGames.Count < (maxListings - resultCount))
                {
                    compatibleGames.Add(game);
                }
            }
        }

        foreach (var game in compatibleGames)
        {
            yield return game;

            if (++resultCount == maxListings)
            {
                yield break;
            }
        }
    }

    private static bool IsGameDesired(IGame game, int map, int impostorCount, GameKeywords language)
    {
        if ((map & (1 << (int)game.Options.Map)) == 0)
        {
            return false;
        }

        if (language != game.Options.Keywords)
        {
            return false;
        }

        if (impostorCount != 0 && game.Options.NumImpostors != impostorCount)
        {
            return false;
        }

        return true;
    }
}
