using System.Threading.Tasks;

namespace Impostor.Server.Account
{
    /// <summary>
    /// Looks up the friend code for a player when a player is bound.
    /// </summary>
    public interface IFriendCodeResolver
    {
        /// <summary>
        /// Looks up the friend code of a player.
        /// </summary>
        /// <param name="puid">Product user id of the player.</param>
        /// <param name="bearerToken">Login token of the player, if any.</param>
        /// <returns>The friend code, or null when unknown.</returns>
        ValueTask<string?> ResolveAsync(string puid, string? bearerToken);
    }
}
