using System;
using Impostor.Api.Games;
using Microsoft.AspNetCore.Http;

namespace Impostor.Api.Http;

/// <summary>
/// Register a method to filter listings on.
/// </summary>
public interface IListingFilter
{
    /// <summary>
    /// Return a filter to filter listings on.
    /// </summary>
    /// <param name="context">HTTP Context of this request.</param>
    /// <returns>A function that looks at a game and returns true iff the connecting player is compatible with this game.</returns>
    Func<IGame, bool> GetFilter(HttpContext context);
}
