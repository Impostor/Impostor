using Impostor.Api.Games;
using Impostor.Api.Net;

namespace Impostor.Api.Events
{
    /// <summary>
    ///     Called just before a <see cref="IClientPlayer"/> joins a game.
    /// </summary>
    public interface IGamePlayerJoiningEvent : IGameEvent
    {
        IClientPlayer Player { get; }

        GameJoinResult? JoinResult { get; set; }
    }
}
