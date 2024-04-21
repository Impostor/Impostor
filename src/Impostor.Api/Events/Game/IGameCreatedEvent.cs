using Impostor.Api.Games;
using Impostor.Api.Net;

namespace Impostor.Api.Events
{
    /// <summary>
    ///     Called whenever a new <see cref="IGame" /> is created.
    /// </summary>
    /// <remarks>
    ///     Note that the game just has been created, so no players have joined
    ///     it yet. If you want to know the future host of this game, use the
    ///     <see cref="Host"/> property.
    /// </remarks>
    public interface IGameCreatedEvent : IGameEvent
    {
        /// <summary>
        ///     Gets the client that requested creation of the game.
        /// </summary>
        /// <remarks>
        ///     Will be null if game creation was requested by a plugin.
        /// </remarks>
        IClient? Host { get; }
    }
}
