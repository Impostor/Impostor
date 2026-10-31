namespace Impostor.Api.Config
{
    public class AccountConfig
    {
        public const string Section = "Account";

        /// <summary>
        /// Gets or sets a value indicating whether PUID/FriendCode binding through temporary game codes is active.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets how long an issued temporary game code stays valid, in seconds.
        /// </summary>
        public int GameCodeLifetimeSeconds { get; set; } = 180;

        /// <summary>
        /// Gets or sets how long a token session is kept, in minutes.
        /// </summary>
        public int SessionLifetimeMinutes { get; set; } = 30;

        /// <summary>
        /// Gets or sets a value indicating whether players that could not be bound to an account are disconnected.
        /// </summary>
        public bool RejectUnmatched { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether friend codes are fetched from the backend when a token is requested.
        /// </summary>
        public bool ResolveFriendCode { get; set; } = true;

        /// <summary>
        /// Gets or sets the endpoint that returns the friend code of the token owner.
        /// The player's login token is sent to it, so only point it at a backend you trust.
        /// </summary>
        public string FriendCodeEndpoint { get; set; } = "https://backend.innersloth.com/api/user/username";

        /// <summary>
        /// Gets or sets how long a fetched friend code is cached, in minutes.
        /// </summary>
        public int FriendCodeCacheMinutes { get; set; } = 30;

        /// <summary>
        /// Gets or sets the timeout of the friend code request, in seconds.
        /// </summary>
        public int FriendCodeTimeoutSeconds { get; set; } = 5;
    }
}
