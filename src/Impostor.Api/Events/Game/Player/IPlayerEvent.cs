using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events.Player
{
    public interface IPlayerEvent : IGameEvent
    {
        /// <summary>
        ///     Gets the <see cref="IClientPlayer" /> that triggered this <see cref="IPlayerEvent" />.
        /// </summary>
        IClientPlayer ClientPlayer { get; }

        /// <summary>
        ///     Gets the networked <see cref="IInnerPlayerControl" /> that triggered this <see cref="IPlayerEvent" />.
        ///     This <see cref="IInnerPlayerControl" /> belongs to the <see cref="IClientPlayer" />.
        /// </summary>
        IInnerPlayerControl PlayerControl { get; }
    }
}
