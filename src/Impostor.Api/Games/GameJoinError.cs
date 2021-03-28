namespace Impostor.Api.Games
{
    public enum GameJoinError
    {
        /// <summary>
        ///     No error occured while joining the game.
        /// </summary>
        None,

        /// <summary>
        ///     The client is not registered in the client manager.
        /// </summary>
        InvalidClient,

        /// <summary>
        ///     The client has been banned from the game.
        /// </summary>
        Banned,

        /// <summary>
        ///     The game is full.
        /// </summary>
        GameFull,

        /// <summary>
        ///     The limbo state of the player is incorrect.
        /// </summary>
        InvalidLimbo,

        /// <summary>
        ///     The game is already started.
        /// </summary>
        GameStarted,

        /// <summary>
        ///     The game has been destroyed.
        /// </summary>
        GameDestroyed,

        /// <summary>
        ///     Custom error by a plugin.
        /// </summary>
        /// <remarks>
        ///     A custom message can be set in <see cref="GameJoinResult.Message" />.
        /// </remarks>
        Custom,
    }
}
