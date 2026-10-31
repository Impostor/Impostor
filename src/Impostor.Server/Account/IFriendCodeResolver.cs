using System.Threading.Tasks;

namespace Impostor.Server.Account
{
    /// <summary>
    /// Looks up the friend code for a player when a token is requested.
    /// </summary>
    public interface IFriendCodeResolver
    {
        /// <summary>
        /// Looks up the friend code of the player behind a token request.
        /// </summary>
        /// <param name="puid">Product user id from the token request.</param>
        /// <param name="bearerToken">Bearer value of the token request, if any.</param>
        /// <returns>The friend code, or null when unknown.</returns>
        ValueTask<string?> ResolveAsync(string puid, string? bearerToken);
    }
}
