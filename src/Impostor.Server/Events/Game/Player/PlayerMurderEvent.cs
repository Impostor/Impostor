using Impostor.Api.Events.Player;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Events.Player;

public class PlayerMurderEvent(
    IGame game,
    IClientPlayer clientPlayer,
    IInnerPlayerControl playerControl,
    IInnerPlayerControl victim,
    MurderResultFlags result)
    : IPlayerMurderEvent
{
    public IGame Game { get; } = game;

    public IClientPlayer ClientPlayer { get; } = clientPlayer;

    public IInnerPlayerControl PlayerControl { get; } = playerControl;

    public IInnerPlayerControl Victim { get; } = victim;

    public MurderResultFlags Result { get; } = result;
}
