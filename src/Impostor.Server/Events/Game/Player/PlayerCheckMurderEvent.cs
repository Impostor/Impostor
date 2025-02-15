using Impostor.Api.Events.Player;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Events.Player;

public class PlayerCheckMurderEvent(
    IGame game,
    IClientPlayer clientPlayer,
    IInnerPlayerControl playerControl,
    IInnerPlayerControl victim,
    MurderResultFlags result)
    : IPlayerCheckMurderEvent
{
    public IGame Game { get; } = game;

    public IClientPlayer ClientPlayer { get; } = clientPlayer;

    public IInnerPlayerControl PlayerControl { get; } = playerControl;

    public IInnerPlayerControl Victim { get; } = victim;

    public MurderResultFlags Result { get; set; } = result;

    public bool IsCancelled { get; set; }
}
