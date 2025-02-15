using Impostor.Api.Events;
using Impostor.Api.Games;
using Impostor.Api.Net;

namespace Impostor.Server.Events;

public class GamePlayerJoinedEvent(IGame game, IClientPlayer player) : IGamePlayerJoinedEvent
{
    public IGame Game { get; } = game;

    public IClientPlayer Player { get; } = player;
}
