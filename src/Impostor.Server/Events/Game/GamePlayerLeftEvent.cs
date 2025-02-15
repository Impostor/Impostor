using Impostor.Api.Events;
using Impostor.Api.Games;
using Impostor.Api.Net;

namespace Impostor.Server.Events;

public class GamePlayerLeftEvent(IGame game, IClientPlayer player, bool isBan) : IGamePlayerLeftEvent
{
    public IGame Game { get; } = game;

    public IClientPlayer Player { get; } = player;

    public bool IsBan { get; } = isBan;
}
