using Impostor.Api.Events.Player;
using Impostor.Api.Games;
using Impostor.Api.Innersloth.Maps;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Events.Player;

public class PlayerEnterVentEvent(
    IGame game,
    IClientPlayer sender,
    IInnerPlayerControl innerPlayerPhysics,
    VentData vent)
    : IPlayerEnterVentEvent
{
    public IGame Game { get; } = game;

    public IClientPlayer ClientPlayer { get; } = sender;

    public IInnerPlayerControl PlayerControl { get; } = innerPlayerPhysics;

    public VentData Vent { get; } = vent;
}
