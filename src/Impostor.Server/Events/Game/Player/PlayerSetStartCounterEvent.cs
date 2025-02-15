using Impostor.Api.Events.Player;
using Impostor.Api.Games;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Events.Player;

public class PlayerSetStartCounterEvent(
    IGame game,
    IClientPlayer clientPlayer,
    IInnerPlayerControl playerControl,
    byte secondsLeft)
    : IPlayerSetStartCounterEvent
{
    public byte SecondsLeft { get; } = secondsLeft;

    public IClientPlayer ClientPlayer { get; } = clientPlayer;

    public IInnerPlayerControl PlayerControl { get; } = playerControl;

    public IGame Game { get; } = game;
}
