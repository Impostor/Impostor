using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events.Player
{
    public interface IPlayerWatchingCameraEvent : IPlayerEvent
    {
        /// <summary>
        ///     Gets the type of action - Started or Stopped watching
        /// </summary>
        bool IsWatching { get; }
    }
}
