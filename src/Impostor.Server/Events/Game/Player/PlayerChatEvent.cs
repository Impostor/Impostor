using Impostor.Api.Events.Player;
using Impostor.Api.Games;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Events.Player;

public class PlayerChatEvent(IGame game, IClientPlayer clientPlayer, IInnerPlayerControl playerControl, string message)
    : IPlayerChatEvent
{
    public IGame Game { get; } = game;

    public IClientPlayer ClientPlayer { get; } = clientPlayer;

    public IInnerPlayerControl PlayerControl { get; } = playerControl;

    public string Message { get; } = message;

    public bool IsCancelled { get; set; }
}
